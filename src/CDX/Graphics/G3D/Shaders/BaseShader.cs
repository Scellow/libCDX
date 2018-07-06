using System;
using System.Collections.Generic;
using CDX.Utils;
using OpenTK;

namespace CDX.Graphics.G3D.Shaders
{
    public abstract class BaseShader : Shader
    {
        public interface Validator
        {
            /** @return True if the input is valid for the renderable, false otherwise. */
            bool validate(BaseShader shader, int inputID, Renderable renderable);
        }

        public interface Setter
        {
            /** @return True if the uniform only has to be set once per render call, false if the uniform must be set for each renderable. */
            bool isGlobal(BaseShader shader, int inputID);

            void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes);
        }

        public abstract class GlobalSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return true;
            }

            public abstract void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes);
        }

        public abstract class LocalSetter : Setter
        {
            public bool isGlobal(BaseShader shader, int inputID)
            {
                return false;
            }

            public abstract void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes);
        }

        public class Uniform : Validator
        {
            public readonly string alias;
            public readonly long   materialMask;
            public readonly long   environmentMask;
            public readonly long   overallMask;

            public Uniform(string alias, long materialMask, long environmentMask, long overallMask)
            {
                this.alias           = alias;
                this.materialMask    = materialMask;
                this.environmentMask = environmentMask;
                this.overallMask     = overallMask;
            }

            public Uniform(string alias, long materialMask = 0, long environmentMask = 0) : this(alias, materialMask, environmentMask, 0)
            {
            }

            public Uniform(string alias, long overallMask) : this(alias, 0, 0, overallMask)
            {
            }

            public bool validate(BaseShader shader, int inputID, Renderable renderable)
            {
                long matFlags = (renderable != null && renderable.material != null) ? renderable.material.getMask() : 0;
                long envFlags = (renderable != null && renderable.environment != null) ? renderable.environment.getMask() : 0;
                return ((matFlags & materialMask) == materialMask) && ((envFlags & environmentMask) == environmentMask)
                                                                   && (((matFlags | envFlags) & overallMask) == overallMask);
            }
        }
	    
	    public class GlobalSetterImpl : GlobalSetter
	    {
		    public Action<BaseShader, int, Renderable, Attributes> cb;

		    public GlobalSetterImpl(Action<BaseShader, int, Renderable, Attributes> cb)
		    {
			    this.cb = cb;
		    }

		    public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
		    {
			    cb?.Invoke(shader, inputID, renderable, combinedAttributes);
		    }
	    }
	    public class LocalSetterImpl : LocalSetter
	    {
		    public Action<BaseShader, int, Renderable, Attributes> cb;

		    public LocalSetterImpl(Action<BaseShader, int, Renderable, Attributes> cb)
		    {
			    this.cb = cb;
		    }

		    public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
		    {
			    cb?.Invoke(shader, inputID, renderable, combinedAttributes);
		    }
	    }
	    

        private readonly List<string>         uniforms   = new List<string>();
        private readonly List<Validator>      validators = new List<Validator>();
        private readonly List<Setter>         setters    = new List<Setter>();
        private          int[]                locations;
        private readonly List<int>            globalUniforms = new List<int>();
        private readonly List<int>            localUniforms  = new List<int>();
        private readonly Dictionary<int, int> attributes     = new Dictionary<int, int>();

        public  ShaderProgram program;
        public  RenderContext context;
        public  Camera        camera;
        private Mesh          currentMesh;


        public int register(string alias, Validator validator, Setter setter)
        {
            if (locations != null) throw new Exception("Cannot register an uniform after initialization");
            int existing = getUniformID(alias);
            if (existing >= 0)
            {
                validators[existing] = validator;
                setters[existing]    = setter;
                return existing;
            }

            uniforms.Add(alias);
            validators.Add(validator);
            setters.Add(setter);
            return uniforms.Count - 1;
        }

        public int register(string alias, Validator validator)
        {
            return register(alias, validator, null);
        }

        public int register(string alias, Setter setter)
        {
            return register(alias, null, setter);
        }

        public int register(string alias)
        {
            return register(alias, null, null);
        }

        public int register(Uniform uniform, Setter setter)
        {
            return register(uniform.alias, uniform, setter);
        }

        public int register(Uniform uniform)
        {
            return register(uniform, null);
        }


        public int getUniformID(string alias)
        {
            int n = uniforms.Count;
            for (int i = 0; i < n; i++)
                if (uniforms[i] == alias)
                    return i;
            return -1;
        }

        public string getUniformAlias(int id)
        {
            return uniforms[id];
        }

        public abstract void init();

        public abstract int compareTo(Shader other);

        public abstract bool canRender(Renderable instance);


        public void init(ShaderProgram program, Renderable renderable)
        {
            if (locations != null) throw new Exception("Already initialized");
            if (!program.isCompiled_()) throw new Exception(program.getLog());
            this.program = program;

            int n = uniforms.Count;
            locations = new int[n];
            for (int i = 0; i < n; i++)
            {
                string    input     = uniforms[i];
                Validator validator = validators[i];
                Setter    setter    = setters[i];
                if (validator != null && !validator.validate(this, i, renderable))
                    locations[i] = -1;
                else
                {
                    locations[i] = program.fetchUniformLocation(input, false);
                    if (locations[i] >= 0 && setter != null)
                    {
                        if (setter.isGlobal(this, i))
                            globalUniforms.Add(i);
                        else
                            localUniforms.Add(i);
                    }
                }

                if (locations[i] < 0)
                {
                    validators[i] = null;
                    setters[i]    = null;
                }
            }

            if (renderable != null)
            {
                VertexAttributes attrs = renderable.meshPart.mesh.getVertexAttributes();
                int              c     = attrs.size();
                for (int i = 0; i < c; i++)
                {
                    VertexAttribute attr     = attrs[i];
                    int             location = program.getAttributeLocation(attr.alias);
                    if (location >= 0) attributes.Add(attr.getKey(), location);
                }
            }
        }


        public virtual void begin(Camera camera, RenderContext context)
        {
            this.camera  = camera;
            this.context = context;
            program.begin();
            currentMesh = null;
            for (int u, i = 0; i < globalUniforms.Count; ++i)
            {
                if (setters[u = globalUniforms[i]] != null)
                {
                    setters[u].set(this, u, null, null);
                }
            }
        }

        private Attributes combinedAttributes = new Attributes();
	    private int[] tempArray = new int[0];

	    private int[] getAttributeLocations (VertexAttributes attrs) {
		    
		    int n = attrs.size();
		    if(tempArray.Length != n) tempArray = new int[n]; // todo: maybe implement intarray properly ?
		    
		    for (int i = 0; i < n; i++)
		    {
			    var key = attrs[i].getKey();
			    var tmp = -1;
			    if (attributes.ContainsKey(key))
				    tmp = attributes[key];

			    tempArray[i] = tmp;
		    }
		    return tempArray;
	    }

        public void render(Renderable renderable)
        {
            if (renderable.worldTransform.det3x3() == 0) return;
            combinedAttributes.clear();
            if (renderable.environment != null) combinedAttributes.set(renderable.environment);
            if (renderable.material != null) combinedAttributes.set(renderable.material);
            render(renderable, combinedAttributes);
        }
	    
        public virtual void render(Renderable renderable, Attributes combinedAttributes)
        {
            for (int u, i = 0; i < localUniforms.Count; ++i)
                if (setters[(u = localUniforms[i])] != null)
                    setters[u].set(this, u, renderable, combinedAttributes);
            if (currentMesh != renderable.meshPart.mesh)
            {
                if (currentMesh != null) currentMesh.unbind(program, tempArray);
                currentMesh = renderable.meshPart.mesh;
                currentMesh.bind(program, getAttributeLocations(renderable.meshPart.mesh.getVertexAttributes()));
            }

            renderable.meshPart.render(program, false);
        }


        public virtual void end()
        {
            if (currentMesh != null) {
                currentMesh.unbind(program, tempArray);
                currentMesh = null;
            }
            program.end();
        }

        public void Dispose()
        {
            program = null;
            uniforms.Clear();
            validators.Clear();
            setters.Clear();
            localUniforms.Clear();
            globalUniforms.Clear();
            locations = null;
        }
        
        
        
        
        
        
        
        
        
        
	/** Whether this Shader instance implements the specified uniform, only valid after a call to init(). */
	public bool has (int inputID) {
		return inputID >= 0 && inputID < locations.Length && locations[inputID] >= 0;
	}

	public int loc (int inputID) {
		return (inputID >= 0 && inputID < locations.Length) ? locations[inputID] : -1;
	}

	public bool set (int uniform, Matrix4 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformMatrix(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, Matrix3 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformMatrix(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, Vector3 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, Vector2 value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, Color value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, float value) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, float v1, float v2) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], v1, v2);
		return true;
	}

	public bool set (int uniform, float v1, float v2, float v3) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], v1, v2, v3);
		return true;
	}

	public bool set (int uniform, float v1, float v2, float v3, float v4) {
		if (locations[uniform] < 0) return false;
		program.setUniformf(locations[uniform], v1, v2, v3, v4);
		return true;
	}

	public bool set (int uniform, int value) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], value);
		return true;
	}

	public bool set (int uniform, int v1, int v2) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], v1, v2);
		return true;
	}

	public bool set (int uniform, int v1, int v2, int v3) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], v1, v2, v3);
		return true;
	}

	public bool set (int uniform, int v1, int v2, int v3, int v4) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], v1, v2, v3, v4);
		return true;
	}

	public bool set (int uniform, TextureDescriptor textureDesc) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], context.textureBinder.bind(textureDesc));
		return true;
	}

	public bool set (int uniform, GLTexture texture) {
		if (locations[uniform] < 0) return false;
		program.setUniformi(locations[uniform], context.textureBinder.bind(texture));
		return true;
	}
    }
}