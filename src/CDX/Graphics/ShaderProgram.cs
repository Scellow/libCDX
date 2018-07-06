using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics
{
    public class ShaderProgram
    {
        public static readonly string POSITION_ATTRIBUTE   = "a_position";
        public static readonly string NORMAL_ATTRIBUTE     = "a_normal";
        public static readonly string COLOR_ATTRIBUTE      = "a_color";
        public static readonly string TEXCOORD_ATTRIBUTE   = "a_texCoord";
        public static readonly string TANGENT_ATTRIBUTE    = "a_tangent";
        public static readonly string BINORMAL_ATTRIBUTE   = "a_binormal";
        public static readonly string BONEWEIGHT_ATTRIBUTE = "a_boneWeight";

        public static bool pedantic = true;

        public static string prependVertexCode   = "";
        public static string prependFragmentCode = "";

        private string log = "";
        private bool   isCompiled;

        private readonly Dictionary<string, int> uniforms = new Dictionary<string, int>();

        private readonly Dictionary<string, ActiveUniformType> uniformTypes = new Dictionary<string, ActiveUniformType>();

        private readonly Dictionary<string, int> uniformSizes = new Dictionary<string, int>();

        private string[] uniformNames;

        private readonly Dictionary<string, int> attributes = new Dictionary<string, int>();

        private readonly Dictionary<string, ActiveAttribType> attributeTypes = new Dictionary<string, ActiveAttribType>();

        private readonly Dictionary<string, int> attributeSizes = new Dictionary<string, int>();

        private string[] attributeNames;

        private int program;

        private int vertexShaderHandle;

        private int fragmentShaderHandle;


        private readonly string vertexShaderSource;

        private readonly string fragmentShaderSource;

        private bool invalidated;

        private int refCount = 0;

        public ShaderProgram(string vertexShader, string fragmentShader)
        {
            if (vertexShader == null) throw new Exception("vertex shader must not be null");
            if (fragmentShader == null) throw new Exception("fragment shader must not be null");

            if (!string.IsNullOrEmpty(prependVertexCode))
                vertexShader = prependVertexCode + vertexShader;
            if (!string.IsNullOrEmpty(prependFragmentCode))
                fragmentShader = prependFragmentCode + fragmentShader;

            this.vertexShaderSource   = vertexShader;
            this.fragmentShaderSource = fragmentShader;

            compileShaders(vertexShader, fragmentShader);
            if (isCompiled_())
            {
                fetchAttributes();
                fetchUniforms();
                //addManagedShader(Gdx.app, this);
            }
        }

        private void compileShaders(string vertexShader, string fragmentShader)
        {
            vertexShaderHandle   = loadShader(ShaderType.VertexShader, vertexShader);
            fragmentShaderHandle = loadShader(ShaderType.FragmentShader, fragmentShader);

            if (vertexShaderHandle == -1 || fragmentShaderHandle == -1)
            {
                isCompiled = false;
                return;
            }

            program = linkProgram(createProgram());
            if (program == -1)
            {
                isCompiled = false;
                return;
            }

            isCompiled = true;
        }

        private int loadShader(ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            if (shader == 0) return -1;

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            // todo: check what this is, not present in GL4
            //GL.GetShaderiv(shader, GL20.GL_COMPILE_STATUS, intbuf);

            var infoLog = GL.GetShaderInfoLog(shader);
            if (!string.IsNullOrEmpty(infoLog))
            {
                log += type == ShaderType.VertexShader ? "Vertex shader\n" : "Fragment shader:\n";
                log += infoLog;
                return -1;
            }

            return shader;
        }

        protected int createProgram()
        {
            int program = GL.CreateProgram();
            return program != 0 ? program : -1;
        }


        private int linkProgram(int program)
        {
            if (program == -1) return -1;

            GL.AttachShader(program, vertexShaderHandle);
            GL.AttachShader(program, fragmentShaderHandle);
            GL.LinkProgram(program);

            var log = GL.GetProgramInfoLog(program);
            if (!string.IsNullOrEmpty(log))
            {
                this.log = log;
                return -1;
            }

            return program;
        }

        public string getLog()
        {
            if (isCompiled)
            {
                log = GL.GetProgramInfoLog(program);
                return log;
            }
            else
            {
                return log;
            }
        }

        public bool isCompiled_()
        {
            return isCompiled;
        }

        private int fetchAttributeLocation(string name)
        {
            // -2 == not yet cached
            // -1 == cached but not found

            int location;

            if (attributes.ContainsKey(name) == false) location = -2;
            else location                                       = attributes[name];

            if (location == -2)
            {
                location = GL.GetAttribLocation(program, name);
                attributes.Add(name, location);
            }

            return location;
        }

        private int fetchUniformLocation(string name)
        {
            return fetchUniformLocation(name, pedantic);
        }

        public int fetchUniformLocation(string name, bool pedantic)
        {
            // -2 == not yet cached
            // -1 == cached but not found

            int location;

            if (uniforms.ContainsKey(name) == false) location = -2;
            else location                                     = uniforms[name];

            if (location == -2)
            {
                location = GL.GetUniformLocation(program, name);
                if (location == -1 && pedantic) throw new Exception("no uniform with name '" + name + "' in shader");
                uniforms.Add(name, location);
            }

            return location;
        }

        public void setUniformi(string name, int value)
        {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform1(location, value);
        }

        public void setUniformi(int location, int value)
        {
            checkManaged();
            GL.Uniform1(location, value);
        }

        public void setUniformi(string name, int value1, int value2)
        {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform2(location, value1, value2);
        }

        public void setUniformi(int location, int value1, int value2)
        {
            checkManaged();
            GL.Uniform2(location, value1, value2);
        }

        public void setUniformi(string name, int value1, int value2, int value3)
        {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform3(location, value1, value2, value3);
        }

        public void setUniformi(int location, int value1, int value2, int value3)
        {
            checkManaged();
            GL.Uniform3(location, value1, value2, value3);
        }
        public void setUniformi(int location, int value1, int value2, int value3, int value4)
        {
            checkManaged();
            GL.Uniform4(location, value1, value2, value3, value4);
        }

        // other set

        public void setUniformMatrix(string name, Matrix4 matrix)
        {
            setUniformMatrix(name, matrix, false);
        }

        public void setUniformMatrix(string name, Matrix4 matrix, bool transpose)
        {
            setUniformMatrix(fetchUniformLocation(name), matrix, transpose);
        }

        public void setVertexAttribute(int location, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
        {
            checkManaged();
            GL.VertexAttribPointer(location, size, type, normalize, stride, offset);
        }

        public void setUniformMatrix(int location, Matrix4 matrix)
        {
            setUniformMatrix(location, matrix, false);
        }

        public void setUniformMatrix(int location, Matrix4 matrix, bool transpose)
        {
            checkManaged();
            GL.UniformMatrix4(location, transpose, ref matrix);
        }


        public void setUniformMatrix(int location, Matrix3 matrix)
        {
            setUniformMatrix(location, matrix, false);
        }

        public void setUniformMatrix(int location, Matrix3 matrix, bool transpose)
        {
            checkManaged();
            GL.UniformMatrix3(location, transpose, ref matrix);
        }
        
        public void setUniformf (String name, Color values) {
            setUniformf(name, values.r, values.g, values.b, values.a);
        }

        public void setUniformf (int location, Color values) {
            setUniformf(location, values.r, values.g, values.b, values.a);
        }
        
        
        public void setUniformf (string name, Vector3 values) {
            setUniformf(name, values.X, values.Y, values.Z);
        }

        public void setUniformf (int location, Vector3 values) {
            setUniformf(location, values.X, values.Y, values.Z);
        }
        
        public void setUniformf (string name, Vector2 values) {
            setUniformf(name, values.X, values.Y);
        }

        public void setUniformf (int location, Vector2 values) {
            setUniformf(location, values.X, values.Y);
        }
        
        
        public void setUniformf (String name, float value1) {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform1(location, value1);
        }

        public void setUniformf (int location, float value1) {
            checkManaged();
            GL.Uniform1(location, value1);
        }
        
        public void setUniformf (String name, float value1, float value2) {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform2(location, value1, value2);
        }

        public void setUniformf (int location, float value1, float value2) {
            checkManaged();
            GL.Uniform2(location, value1, value2);
        }
        
        public void setUniformf (String name, float value1, float value2, float value3) {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform3(location, value1, value2, value3);
        }

        public void setUniformf (int location, float value1, float value2, float value3) {
            checkManaged();
            GL.Uniform3(location, value1, value2, value3);
        }
        public void setUniformf (String name, float value1, float value2, float value3, float value4) {
            checkManaged();
            int location = fetchUniformLocation(name);
            GL.Uniform4(location, value1, value2, value3, value4);
        }

        public void setUniformf (int location, float value1, float value2, float value3, float value4) {
            checkManaged();
            GL.Uniform4(location, value1, value2, value3, value4);
        }

        // todo: check this
        public void setUniform3fv (int location, float[] values) {
            checkManaged();
            GL.Uniform3(location, values.Length, values);
        }
        

        // ----


        public void begin()
        {
            checkManaged();
            GL.UseProgram(program);
        }

        public void end()
        {
            GL.UseProgram(0);
        }

        public void dispose()
        {
            GL.UseProgram(0);
            GL.DeleteShader(vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
            GL.DeleteProgram(program);
            //if (shaders.get(Gdx.app) != null) shaders.get(Gdx.app).removeValue(this, true);
        }

        public void disableVertexAttribute(string name)
        {
            checkManaged();
            int location = fetchAttributeLocation(name);
            if (location == -1) return;
            GL.DisableVertexAttribArray(location);
        }

        public void disableVertexAttribute(int location)
        {
            checkManaged();
            GL.DisableVertexAttribArray(location);
        }

        public void enableVertexAttribute(string name)
        {
            checkManaged();
            int location = fetchAttributeLocation(name);
            if (location == -1) return;
            GL.EnableVertexAttribArray(location);
        }

        public void enableVertexAttribute(int location)
        {
            checkManaged();
            GL.EnableVertexAttribArray(location);
        }

        private void checkManaged()
        {
            if (invalidated)
            {
                compileShaders(vertexShaderSource, fragmentShaderSource);
                invalidated = false;
            }
        }


        private void fetchUniforms()
        {
            GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out var numUniforms);
            uniformNames = new string[numUniforms];

            for (int i = 0; i < numUniforms; i++)
            {
                var name     = GL.GetActiveUniform(program, i, out var size, out var type);
                var location = GL.GetUniformLocation(program, name);
                uniforms.Add(name, location);
                uniformTypes.Add(name, type);
                uniformSizes.Add(name, size);
                uniformNames[i] = name;
            }
        }

        private void fetchAttributes()
        {
            GL.GetProgram(program, GetProgramParameterName.ActiveAttributes, out var numAttributes);

            attributeNames = new string[numAttributes];

            for (int i = 0; i < numAttributes; i++)
            {
                var name     = GL.GetActiveAttrib(program, i, out var size, out var type);
                int location = GL.GetAttribLocation(program, name);
                attributes.Add(name, location);
                attributeTypes.Add(name, type);
                attributeSizes.Add(name, size);
                attributeNames[i] = name;
            }
        }


        public bool hasAttribute(string name)
        {
            return attributes.ContainsKey(name);
        }

        public ActiveAttribType getAttributeType(string name)
        {
            if (attributeTypes.ContainsKey(name))
                return attributeTypes[name];
            return 0;
        }

        public int getAttributeLocation(string name)
        {
            if (attributes.ContainsKey(name)) return attributes[name];
            return -1;
        }
    }
}