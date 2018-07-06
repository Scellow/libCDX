using System;
using System.IO;
using CDX.Graphics.G3D.Environements;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace CDX.Graphics.G3D.Shaders
{
    public class DefaultShader : BaseShader
    {
        public class Config
        {
            /** The uber vertex shader to use, null to use the default vertex shader. */
            public string vertexShader = null;

            /** The uber fragment shader to use, null to use the default fragment shader. */
            public string fragmentShader = null;

            /** The number of directional lights to use */
            public int numDirectionalLights = 2;

            /** The number of point lights to use */
            public int numPointLights = 5;

            /** The number of spot lights to use */
            public int numSpotLights = 0;

            /** The number of bones to use */
            public int numBones = 12;

            /** */
            public bool ignoreUnimplemented = true;

            /** Set to 0 to disable culling, -1 to inherit from {@link DefaultShader#defaultCullFace} */
            public CullFaceMode? defaultCullFace = null;

            /** Set to 0 to disable depth test, -1 to inherit from {@link DefaultShader#defaultDepthFunc} */
            public DepthFunction? defaultDepthFunc = null;

            public Config()
            {
            }

            public Config(string vertexShader, string fragmentShader)
            {
                this.vertexShader   = vertexShader;
                this.fragmentShader = fragmentShader;
            }
        }

        public static class Inputs
        {
            public static readonly Uniform projTrans       = new Uniform("u_projTrans");
            public static readonly Uniform viewTrans       = new Uniform("u_viewTrans");
            public static readonly Uniform projViewTrans   = new Uniform("u_projViewTrans");
            public static readonly Uniform cameraPosition  = new Uniform("u_cameraPosition");
            public static readonly Uniform cameraDirection = new Uniform("u_cameraDirection");
            public static readonly Uniform cameraUp        = new Uniform("u_cameraUp");
            public static readonly Uniform cameraNearFar   = new Uniform("u_cameraNearFar");

            public static readonly Uniform worldTrans         = new Uniform("u_worldTrans");
            public static readonly Uniform viewWorldTrans     = new Uniform("u_viewWorldTrans");
            public static readonly Uniform projViewWorldTrans = new Uniform("u_projViewWorldTrans");
            public static readonly Uniform normalMatrix       = new Uniform("u_normalMatrix");
            public static readonly Uniform bones              = new Uniform("u_bones");

            public static readonly Uniform shininess             = new Uniform("u_shininess", FloatAttribute.Shininess);
            public static readonly Uniform opacity               = new Uniform("u_opacity", BlendingAttribute.Type);
            public static readonly Uniform diffuseColor          = new Uniform("u_diffuseColor", ColorAttribute.Diffuse);
            public static readonly Uniform diffuseTexture        = new Uniform("u_diffuseTexture", TextureAttribute.Diffuse);
            public static readonly Uniform diffuseUVTransform    = new Uniform("u_diffuseUVTransform", TextureAttribute.Diffuse);
            public static readonly Uniform specularColor         = new Uniform("u_specularColor", ColorAttribute.Specular);
            public static readonly Uniform specularTexture       = new Uniform("u_specularTexture", TextureAttribute.Specular);
            public static readonly Uniform specularUVTransform   = new Uniform("u_specularUVTransform", TextureAttribute.Specular);
            public static readonly Uniform emissiveColor         = new Uniform("u_emissiveColor", ColorAttribute.Emissive);
            public static readonly Uniform emissiveTexture       = new Uniform("u_emissiveTexture", TextureAttribute.Emissive);
            public static readonly Uniform emissiveUVTransform   = new Uniform("u_emissiveUVTransform", TextureAttribute.Emissive);
            public static readonly Uniform reflectionColor       = new Uniform("u_reflectionColor", ColorAttribute.Reflection);
            public static readonly Uniform reflectionTexture     = new Uniform("u_reflectionTexture", TextureAttribute.Reflection);
            public static readonly Uniform reflectionUVTransform = new Uniform("u_reflectionUVTransform", TextureAttribute.Reflection);
            public static readonly Uniform normalTexture         = new Uniform("u_normalTexture", TextureAttribute.Normal);
            public static readonly Uniform normalUVTransform     = new Uniform("u_normalUVTransform", TextureAttribute.Normal);
            public static readonly Uniform ambientTexture        = new Uniform("u_ambientTexture", TextureAttribute.Ambient);
            public static readonly Uniform ambientUVTransform    = new Uniform("u_ambientUVTransform", TextureAttribute.Ambient);
            public static readonly Uniform alphaTest             = new Uniform("u_alphaTest");

            public static readonly Uniform ambientCube        = new Uniform("u_ambientCubemap");
            public static readonly Uniform dirLights          = new Uniform("u_dirLights");
            public static readonly Uniform pointLights        = new Uniform("u_pointLights");
            public static readonly Uniform spotLights         = new Uniform("u_spotLights");
            public static readonly Uniform environmentCubemap = new Uniform("u_environmentCubemap");
        }

        public static class Setters
        {
            public static readonly Setter projTrans     = new GlobalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.projection); });
            public static readonly Setter viewTrans     = new GlobalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.view); });
            public static readonly Setter projViewTrans = new GlobalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.combined); });

            public static readonly Setter cameraPosition = new GlobalSetterImpl((shader, id, renderable, attributes) =>
            {
                shader.set(id, shader.camera.position.X, shader.camera.position.Y, shader.camera.position.Z,
                    1.1881f / (shader.camera.far * shader.camera.far));
            });

            public static readonly Setter cameraDirection = new GlobalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.direction); });
            public static readonly Setter cameraUp        = new GlobalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.up); });
            public static readonly Setter cameraNearFar   = new GlobalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.near, shader.camera.far); });

            public static readonly Setter worldTrans     = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, renderable.worldTransform); });
            public static readonly Setter viewWorldTrans = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.view * renderable.worldTransform); });

            public static readonly Setter projViewWorldTrans = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, shader.camera.combined * renderable.worldTransform); });

            // todo: double check transpose
            public static readonly Setter normalMatrix = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, Matrix4.Transpose(Matrix4.Invert(renderable.worldTransform))); });


            public class Bones : LocalSetter
            {
                private static readonly Matrix4 idtMatrix = Matrix4.Identity;
                public readonly         float[] bones;

                public Bones(int numBones)
                {
                    this.bones = new float[numBones * 16];
                }

                public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
                {
                    // todo: figure out this
                    //for (int i = 0; i < bones.Length; i++)
                    //{
                    //    int idx = i / 16;
                    //    bones[i] = (renderable.bones == null || idx >= renderable.bones.Length || renderable.bones[idx] == null)
                    //        ? idtMatrix.val[i % 16]
                    //        : renderable.bones[idx].val[i % 16];
                    //}
                    //shader.program.setUniformMatrix(shader.loc(inputID), bones, 0, bones.Length);
                }
            }

            public static readonly Setter shininess = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, attributes.get<FloatAttribute>(FloatAttribute.Shininess).value); });


            // DIFFUSE            
            public static readonly Setter diffuseColor = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, attributes.get<ColorAttribute>(ColorAttribute.Diffuse).color); });

            public static readonly Setter diffuseTexture = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var unit = shader.context.textureBinder.bind(attributes.get<TextureAttribute>(TextureAttribute.Diffuse).textureDescription);
                shader.set(id, unit);
            });

            public static readonly Setter diffuseUVTransform = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var ta = attributes.get<TextureAttribute>(TextureAttribute.Diffuse);
                shader.set(id, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            });

            // SPECULAR
            public static readonly Setter specularColor = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, attributes.get<ColorAttribute>(ColorAttribute.Specular).color); });

            public static readonly Setter specularTexture = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var unit = shader.context.textureBinder.bind(attributes.get<TextureAttribute>(TextureAttribute.Specular).textureDescription);
                shader.set(id, unit);
            });

            public static readonly Setter specularUVTransform = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var ta = attributes.get<TextureAttribute>(TextureAttribute.Specular);
                shader.set(id, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            });

            // EMISSIVE
            public static readonly Setter emissiveColor = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, attributes.get<ColorAttribute>(ColorAttribute.Emissive).color); });

            public static readonly Setter emissiveTexture = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var unit = shader.context.textureBinder.bind(attributes.get<TextureAttribute>(TextureAttribute.Emissive).textureDescription);
                shader.set(id, unit);
            });

            public static readonly Setter emissiveUVTransform = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var ta = attributes.get<TextureAttribute>(TextureAttribute.Emissive);
                shader.set(id, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            });

            // REFLECTION
            public static readonly Setter reflectionColor = new LocalSetterImpl((shader, id, renderable, attributes) => { shader.set(id, attributes.get<ColorAttribute>(ColorAttribute.Reflection).color); });

            public static readonly Setter reflectionTexture = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var unit = shader.context.textureBinder.bind(attributes.get<TextureAttribute>(TextureAttribute.Reflection).textureDescription);
                shader.set(id, unit);
            });

            public static readonly Setter reflectionUVTransform = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var ta = attributes.get<TextureAttribute>(TextureAttribute.Reflection);
                shader.set(id, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            });

            // NORMAL
            public static readonly Setter normalTexture = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var unit = shader.context.textureBinder.bind(attributes.get<TextureAttribute>(TextureAttribute.Normal).textureDescription);
                shader.set(id, unit);
            });

            public static readonly Setter normalUVTransform = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var ta = attributes.get<TextureAttribute>(TextureAttribute.Normal);
                shader.set(id, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            });

            // AMBIANT
            public static readonly Setter ambientTexture = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var unit = shader.context.textureBinder.bind(attributes.get<TextureAttribute>(TextureAttribute.Ambient).textureDescription);
                shader.set(id, unit);
            });

            public static readonly Setter ambientUVTransform = new LocalSetterImpl((shader, id, renderable, attributes) =>
            {
                var ta = attributes.get<TextureAttribute>(TextureAttribute.Ambient);
                shader.set(id, ta.offsetU, ta.offsetV, ta.scaleU, ta.scaleV);
            });

            public class ACubemap : LocalSetter
            {
                private readonly static float[] ones = new float[]
                {
                    1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
                };

                private readonly AmbientCubemap cacheAmbientCubemap = new AmbientCubemap();
                public readonly  int            dirLightsOffset;
                public readonly  int            pointLightsOffset;

                public ACubemap(int dirLightsOffset, int pointLightsOffset)
                {
                    this.dirLightsOffset   = dirLightsOffset;
                    this.pointLightsOffset = pointLightsOffset;
                }

                public override void set(BaseShader shader, int inputID, Renderable renderable, Attributes combinedAttributes)
                {
                    if (renderable.environment == null)
                        shader.program.setUniform3fv(shader.loc(inputID), ones);
                    else
                    {
                        // todo: double check
                        var tmpV1 = renderable.worldTransform.ExtractTranslation();
                        if (combinedAttributes.has(ColorAttribute.AmbientLight))
                            cacheAmbientCubemap.set(((ColorAttribute) combinedAttributes.get(ColorAttribute.AmbientLight)).color);

                        if (combinedAttributes.has(DirectionalLightsAttribute.Type))
                        {
                            var lights = combinedAttributes.get<DirectionalLightsAttribute>(DirectionalLightsAttribute.Type).lights;
                            for (int i = dirLightsOffset; i < lights.Count; i++)
                                cacheAmbientCubemap.add(lights[i].color, lights[i].direction);
                        }

                        if (combinedAttributes.has(PointLightsAttribute.Type))
                        {
                            var lights = (combinedAttributes.get<PointLightsAttribute>(PointLightsAttribute.Type)).lights;
                            for (int i = pointLightsOffset; i < lights.Count; i++)
                                cacheAmbientCubemap.add(lights[i].color, lights[i].position, tmpV1, lights[i].intensity);
                        }

                        cacheAmbientCubemap.clamp();
                        shader.program.setUniform3fv(shader.loc(inputID), cacheAmbientCubemap.data);
                    }
                }
            }
        }


        protected static long implementedFlags = BlendingAttribute.Type | TextureAttribute.Diffuse | ColorAttribute.Diffuse
                                                 | ColorAttribute.Specular | FloatAttribute.Shininess;


        // Global uniforms
        public readonly int u_projTrans;
        public readonly int u_viewTrans;
        public readonly int u_projViewTrans;
        public readonly int u_cameraPosition;
        public readonly int u_cameraDirection;
        public readonly int u_cameraUp;
        public readonly int u_cameraNearFar;

        public readonly int u_time;

        // Object uniforms
        public readonly int u_worldTrans;
        public readonly int u_viewWorldTrans;
        public readonly int u_projViewWorldTrans;
        public readonly int u_normalMatrix;

        public readonly int u_bones;

        // Material uniforms
        public readonly int u_shininess;
        public readonly int u_opacity;
        public readonly int u_diffuseColor;
        public readonly int u_diffuseTexture;
        public readonly int u_diffuseUVTransform;
        public readonly int u_specularColor;
        public readonly int u_specularTexture;
        public readonly int u_specularUVTransform;
        public readonly int u_emissiveColor;
        public readonly int u_emissiveTexture;
        public readonly int u_emissiveUVTransform;
        public readonly int u_reflectionColor;
        public readonly int u_reflectionTexture;
        public readonly int u_reflectionUVTransform;
        public readonly int u_normalTexture;
        public readonly int u_normalUVTransform;
        public readonly int u_ambientTexture;
        public readonly int u_ambientUVTransform;

        public readonly int u_alphaTest;

        // Lighting uniforms
        protected readonly int u_ambientCubemap;
        protected readonly int u_environmentCubemap;
        protected readonly int u_dirLights0color;
        protected readonly int u_dirLights0direction;
        protected readonly int u_dirLights1color;
        protected readonly int u_pointLights0color;
        protected readonly int u_pointLights0position;
        protected readonly int u_pointLights0intensity;
        protected readonly int u_pointLights1color;
        protected readonly int u_spotLights0color;
        protected readonly int u_spotLights0position;
        protected readonly int u_spotLights0intensity;
        protected readonly int u_spotLights0direction;
        protected readonly int u_spotLights0cutoffAngle;
        protected readonly int u_spotLights0exponent;
        protected readonly int u_spotLights1color;
        protected readonly int u_fogColor;
        protected readonly int u_shadowMapProjViewTrans;
        protected readonly int u_shadowTexture;

        protected readonly int u_shadowPCFOffset;
        // FIXME Cache vertex attribute locations...

        protected int dirLightsLoc;
        protected int dirLightsColorOffset;
        protected int dirLightsDirectionOffset;
        protected int dirLightsSize;
        protected int pointLightsLoc;
        protected int pointLightsColorOffset;
        protected int pointLightsPositionOffset;
        protected int pointLightsIntensityOffset;
        protected int pointLightsSize;
        protected int spotLightsLoc;
        protected int spotLightsColorOffset;
        protected int spotLightsPositionOffset;
        protected int spotLightsDirectionOffset;
        protected int spotLightsIntensityOffset;
        protected int spotLightsCutoffAngleOffset;
        protected int spotLightsExponentOffset;
        protected int spotLightsSize;


        protected readonly bool               lighting;
        protected readonly bool               environmentCubemap;
        protected readonly bool               shadowMap;
        protected readonly AmbientCubemap     ambientCubemap = new AmbientCubemap();
        protected readonly DirectionalLight[] directionalLights;
        protected readonly PointLight[]       pointLights;
        protected readonly SpotLight[]        spotLights;

        /** The renderable used to create this shader, invalid after the call to init */
        private Renderable renderable;

        /** The attributes that this shader supports */
        protected readonly long attributesMask;
        private readonly   long vertexMask;

        protected readonly Config config;

        /** Attributes which are not required but always supported. */
        private readonly static long optionalAttributes = IntAttribute.CullFace | DepthTestAttribute.Type;

        public DefaultShader (Renderable renderable):            this(renderable, new Config()){
        }
        
        public DefaultShader(Renderable renderable, Config config) :
            this(renderable, config, createPrefix(renderable, config))
        {
        }

        public DefaultShader(Renderable renderable, Config config, string prefix)
            : this(renderable, config, prefix, 
                config.vertexShader ?? getDefaultVertexShader(),
                config.fragmentShader ?? getDefaultFragmentShader())
        {
        }


        public DefaultShader(Renderable renderable, Config config, string prefix, string vertexShader,
            string fragmentShader) : this(renderable, config, new ShaderProgram(prefix + vertexShader, prefix + fragmentShader))
        {
        }
        
        
        public DefaultShader(Renderable renderable, Config config, ShaderProgram shaderProgram)
        {
            Attributes attributes = combineAttributes(renderable);
            this.config   = config;
            this.program  = shaderProgram;
            this.lighting = renderable.environment != null;

            // todo: implement cubemaps
            /*
            this.environmentCubemap = attributes.has(CubemapAttribute.EnvironmentMap)
                                      || (lighting && attributes.has(CubemapAttribute.EnvironmentMap));
            */
            this.shadowMap  = lighting && renderable.environment.shadowMap != null;
            this.renderable = renderable;
            attributesMask  = attributes.getMask() | optionalAttributes;
            vertexMask      = renderable.meshPart.mesh.getVertexAttributes().getMaskWithSizePacked();

            this.directionalLights = new DirectionalLight[lighting && config.numDirectionalLights > 0 ? config.numDirectionalLights : 0];
            for (int i = 0; i < directionalLights.Length; i++)
                directionalLights[i] = new DirectionalLight();
            this.pointLights = new PointLight[lighting && config.numPointLights > 0 ? config.numPointLights : 0];
            for (int i = 0; i < pointLights.Length; i++)
                pointLights[i] = new PointLight();
            this.spotLights = new SpotLight[lighting && config.numSpotLights > 0 ? config.numSpotLights : 0];
            for (int i = 0; i < spotLights.Length; i++)
                spotLights[i] = new SpotLight();

            if (!config.ignoreUnimplemented && (implementedFlags & attributesMask) != attributesMask)
                throw new Exception("Some attributes not implemented yet (" + attributesMask + ")");

            // Global uniforms
            u_projTrans       = register(Inputs.projTrans, Setters.projTrans);
            u_viewTrans       = register(Inputs.viewTrans, Setters.viewTrans);
            u_projViewTrans   = register(Inputs.projViewTrans, Setters.projViewTrans);
            u_cameraPosition  = register(Inputs.cameraPosition, Setters.cameraPosition);
            u_cameraDirection = register(Inputs.cameraDirection, Setters.cameraDirection);
            u_cameraUp        = register(Inputs.cameraUp, Setters.cameraUp);
            u_cameraNearFar   = register(Inputs.cameraNearFar, Setters.cameraNearFar);
            u_time            = register(new Uniform("u_time"));
            // Object uniforms
            u_worldTrans         = register(Inputs.worldTrans, Setters.worldTrans);
            u_viewWorldTrans     = register(Inputs.viewWorldTrans, Setters.viewWorldTrans);
            u_projViewWorldTrans = register(Inputs.projViewWorldTrans, Setters.projViewWorldTrans);
            u_normalMatrix       = register(Inputs.normalMatrix, Setters.normalMatrix);
            u_bones = (renderable.bones != null && config.numBones > 0)
                ? register(Inputs.bones, new Setters.Bones(config.numBones))
                : -1;

            u_shininess             = register(Inputs.shininess, Setters.shininess);
            u_opacity               = register(Inputs.opacity);
            u_diffuseColor          = register(Inputs.diffuseColor, Setters.diffuseColor);
            u_diffuseTexture        = register(Inputs.diffuseTexture, Setters.diffuseTexture);
            u_diffuseUVTransform    = register(Inputs.diffuseUVTransform, Setters.diffuseUVTransform);
            u_specularColor         = register(Inputs.specularColor, Setters.specularColor);
            u_specularTexture       = register(Inputs.specularTexture, Setters.specularTexture);
            u_specularUVTransform   = register(Inputs.specularUVTransform, Setters.specularUVTransform);
            u_emissiveColor         = register(Inputs.emissiveColor, Setters.emissiveColor);
            u_emissiveTexture       = register(Inputs.emissiveTexture, Setters.emissiveTexture);
            u_emissiveUVTransform   = register(Inputs.emissiveUVTransform, Setters.emissiveUVTransform);
            u_reflectionColor       = register(Inputs.reflectionColor, Setters.reflectionColor);
            u_reflectionTexture     = register(Inputs.reflectionTexture, Setters.reflectionTexture);
            u_reflectionUVTransform = register(Inputs.reflectionUVTransform, Setters.reflectionUVTransform);
            u_normalTexture         = register(Inputs.normalTexture, Setters.normalTexture);
            u_normalUVTransform     = register(Inputs.normalUVTransform, Setters.normalUVTransform);
            u_ambientTexture        = register(Inputs.ambientTexture, Setters.ambientTexture);
            u_ambientUVTransform    = register(Inputs.ambientUVTransform, Setters.ambientUVTransform);
            u_alphaTest             = register(Inputs.alphaTest);

            u_ambientCubemap = lighting ? register(Inputs.ambientCube, new Setters.ACubemap(config.numDirectionalLights, config.numPointLights)) : -1;
            // todo: implement cubemap
            //u_environmentCubemap = environmentCubemap ? register(Inputs.environmentCubemap, Setters.environmentCubemap) : -1;

            u_dirLights0color        = register(new Uniform("u_dirLights[0].color"));
            u_dirLights0direction    = register(new Uniform("u_dirLights[0].direction"));
            u_dirLights1color        = register(new Uniform("u_dirLights[1].color"));
            u_pointLights0color      = register(new Uniform("u_pointLights[0].color"));
            u_pointLights0position   = register(new Uniform("u_pointLights[0].position"));
            u_pointLights0intensity  = register(new Uniform("u_pointLights[0].intensity"));
            u_pointLights1color      = register(new Uniform("u_pointLights[1].color"));
            u_spotLights0color       = register(new Uniform("u_spotLights[0].color"));
            u_spotLights0position    = register(new Uniform("u_spotLights[0].position"));
            u_spotLights0intensity   = register(new Uniform("u_spotLights[0].intensity"));
            u_spotLights0direction   = register(new Uniform("u_spotLights[0].direction"));
            u_spotLights0cutoffAngle = register(new Uniform("u_spotLights[0].cutoffAngle"));
            u_spotLights0exponent    = register(new Uniform("u_spotLights[0].exponent"));
            u_spotLights1color       = register(new Uniform("u_spotLights[1].color"));
            u_fogColor               = register(new Uniform("u_fogColor"));
            u_shadowMapProjViewTrans = register(new Uniform("u_shadowMapProjViewTrans"));
            u_shadowTexture          = register(new Uniform("u_shadowTexture"));
            u_shadowPCFOffset        = register(new Uniform("u_shadowPCFOffset"));
        }


        public static string createPrefix(Renderable renderable, Config config)
        {
            Attributes attributes                                                                                 = combineAttributes(renderable);
            String     prefix                                                                                     = "";
            long       attributesMask                                                                             = attributes.getMask();
            long       vertexMask                                                                                 = renderable.meshPart.mesh.getVertexAttributes().getMask();
            if (and(vertexMask, VertexAttributes.Usage.Position)) prefix                                          += "#define positionFlag\n";
            if (or(vertexMask, VertexAttributes.Usage.ColorUnpacked | VertexAttributes.Usage.ColorPacked)) prefix += "#define colorFlag\n";
            if (and(vertexMask, VertexAttributes.Usage.BiNormal)) prefix                                          += "#define binormalFlag\n";
            if (and(vertexMask, VertexAttributes.Usage.Tangent)) prefix                                           += "#define tangentFlag\n";
            if (and(vertexMask, VertexAttributes.Usage.Normal)) prefix                                            += "#define normalFlag\n";
            if (and(vertexMask, VertexAttributes.Usage.Normal) || and(vertexMask, VertexAttributes.Usage.Tangent | VertexAttributes.Usage.BiNormal))
            {
                if (renderable.environment != null)
                {
                    prefix += "#define lightingFlag\n";
                    prefix += "#define ambientCubemapFlag\n";
                    prefix += "#define numDirectionalLights " + config.numDirectionalLights + "\n";
                    prefix += "#define numPointLights " + config.numPointLights + "\n";
                    prefix += "#define numSpotLights " + config.numSpotLights + "\n";
                    if (attributes.has(ColorAttribute.Fog))
                    {
                        prefix += "#define fogFlag\n";
                    }

                    if (renderable.environment.shadowMap != null) prefix += "#define shadowMapFlag\n";
                    // todo: implement cubemap
                    //if (attributes.has(CubemapAttribute.EnvironmentMap)) prefix += "#define environmentCubemapFlag\n";
                }
            }

            int n = renderable.meshPart.mesh.getVertexAttributes().size();
            for (int i = 0; i < n; i++)
            {
                VertexAttribute attr = renderable.meshPart.mesh.getVertexAttributes()[i];
                if (attr.usage == VertexAttributes.Usage.BoneWeight)
                    prefix                                                               += "#define boneWeight" + attr.unit + "Flag\n";
                else if (attr.usage == VertexAttributes.Usage.TextureCoordinates) prefix += "#define texCoord" + attr.unit + "Flag\n";
            }

            if ((attributesMask & BlendingAttribute.Type) == BlendingAttribute.Type)
                prefix += "#define " + BlendingAttribute.Alias + "Flag\n";
            if ((attributesMask & TextureAttribute.Diffuse) == TextureAttribute.Diffuse)
            {
                prefix += "#define " + TextureAttribute.DiffuseAlias + "Flag\n";
                prefix += "#define " + TextureAttribute.DiffuseAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
            }

            if ((attributesMask & TextureAttribute.Specular) == TextureAttribute.Specular)
            {
                prefix += "#define " + TextureAttribute.SpecularAlias + "Flag\n";
                prefix += "#define " + TextureAttribute.SpecularAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
            }

            if ((attributesMask & TextureAttribute.Normal) == TextureAttribute.Normal)
            {
                prefix += "#define " + TextureAttribute.NormalAlias + "Flag\n";
                prefix += "#define " + TextureAttribute.NormalAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
            }

            if ((attributesMask & TextureAttribute.Emissive) == TextureAttribute.Emissive)
            {
                prefix += "#define " + TextureAttribute.EmissiveAlias + "Flag\n";
                prefix += "#define " + TextureAttribute.EmissiveAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
            }

            if ((attributesMask & TextureAttribute.Reflection) == TextureAttribute.Reflection)
            {
                prefix += "#define " + TextureAttribute.ReflectionAlias + "Flag\n";
                prefix += "#define " + TextureAttribute.ReflectionAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
            }

            if ((attributesMask & TextureAttribute.Ambient) == TextureAttribute.Ambient)
            {
                prefix += "#define " + TextureAttribute.AmbientAlias + "Flag\n";
                prefix += "#define " + TextureAttribute.AmbientAlias + "Coord texCoord0\n"; // FIXME implement UV mapping
            }

            if ((attributesMask & ColorAttribute.Diffuse) == ColorAttribute.Diffuse)
                prefix += "#define " + ColorAttribute.DiffuseAlias + "Flag\n";
            if ((attributesMask & ColorAttribute.Specular) == ColorAttribute.Specular)
                prefix += "#define " + ColorAttribute.SpecularAlias + "Flag\n";
            if ((attributesMask & ColorAttribute.Emissive) == ColorAttribute.Emissive)
                prefix += "#define " + ColorAttribute.EmissiveAlias + "Flag\n";
            if ((attributesMask & ColorAttribute.Reflection) == ColorAttribute.Reflection)
                prefix += "#define " + ColorAttribute.ReflectionAlias + "Flag\n";
            if ((attributesMask & FloatAttribute.Shininess) == FloatAttribute.Shininess)
                prefix += "#define " + FloatAttribute.ShininessAlias + "Flag\n";
            if ((attributesMask & FloatAttribute.AlphaTest) == FloatAttribute.AlphaTest)
                prefix += "#define " + FloatAttribute.AlphaTestAlias + "Flag\n";
            if (renderable.bones != null && config.numBones > 0) prefix += "#define numBones " + config.numBones + "\n";
            return prefix;
        }
        
        private static string defaultVertexShader = null;

        public static string getDefaultVertexShader () {
            if (defaultVertexShader == null)
                defaultVertexShader = File.ReadAllText("default.vertex.glsl");
            return defaultVertexShader;
        }

        private static string defaultFragmentShader = null;

        public static string getDefaultFragmentShader ()
        {
            if (defaultFragmentShader == null)
                defaultFragmentShader = File.ReadAllText("default.fragment.glsl");
            return defaultFragmentShader;
        }
        
        

        private static bool and(long mask, long flag)
        {
            return (mask & flag) == flag;
        }

        private static bool or(long mask, long flag)
        {
            return (mask & flag) != 0;
        }

        private static readonly Attributes tmpAttributes = new Attributes();


        // TODO: Perhaps move responsibility for combining attributes to RenderableProvider?
        private static Attributes combineAttributes(Renderable renderable)
        {
            tmpAttributes.clear();
            if (renderable.environment != null) tmpAttributes.set(renderable.environment);
            if (renderable.material != null) tmpAttributes.set(renderable.material);
            return tmpAttributes;
        }

        private static long combineAttributeMasks(Renderable renderable)
        {
            long mask                                = 0;
            if (renderable.environment != null) mask |= renderable.environment.getMask();
            if (renderable.material != null) mask    |= renderable.material.getMask();
            return mask;
        }


        public override void init()
        {
            Gdx.app.debug("DefaultShader", "init");
            ShaderProgram program = this.program;
            this.program = null;
            init(program, renderable);
            renderable = null;

            dirLightsLoc             = loc(u_dirLights0color);
            dirLightsColorOffset     = loc(u_dirLights0color) - dirLightsLoc;
            dirLightsDirectionOffset = loc(u_dirLights0direction) - dirLightsLoc;
            dirLightsSize            = loc(u_dirLights1color) - dirLightsLoc;
            if (dirLightsSize < 0) dirLightsSize = 0;

            pointLightsLoc             = loc(u_pointLights0color);
            pointLightsColorOffset     = loc(u_pointLights0color) - pointLightsLoc;
            pointLightsPositionOffset  = loc(u_pointLights0position) - pointLightsLoc;
            pointLightsIntensityOffset = has(u_pointLights0intensity) ? loc(u_pointLights0intensity) - pointLightsLoc : -1;
            pointLightsSize            = loc(u_pointLights1color) - pointLightsLoc;
            if (pointLightsSize < 0) pointLightsSize = 0;

            spotLightsLoc               = loc(u_spotLights0color);
            spotLightsColorOffset       = loc(u_spotLights0color) - spotLightsLoc;
            spotLightsPositionOffset    = loc(u_spotLights0position) - spotLightsLoc;
            spotLightsDirectionOffset   = loc(u_spotLights0direction) - spotLightsLoc;
            spotLightsIntensityOffset   = has(u_spotLights0intensity) ? loc(u_spotLights0intensity) - spotLightsLoc : -1;
            spotLightsCutoffAngleOffset = loc(u_spotLights0cutoffAngle) - spotLightsLoc;
            spotLightsExponentOffset    = loc(u_spotLights0exponent) - spotLightsLoc;
            spotLightsSize              = loc(u_spotLights1color) - spotLightsLoc;
            if (spotLightsSize < 0) spotLightsSize = 0;
        }

        public override int compareTo(Shader other)
        {
            if (other == null) return -1;
            if (other == this) return 0;
            return 0; // FIXME compare shaders on their impact on performance
        }

        public override bool canRender(Renderable renderable)
        {
            long renderableMask = combineAttributeMasks(renderable);
            return (attributesMask == (renderableMask | optionalAttributes))
                   && (vertexMask == renderable.meshPart.mesh.getVertexAttributes().getMaskWithSizePacked()) && (renderable.environment != null) == lighting;
        }

        public override bool Equals(object obj)
        {
            return (obj is DefaultShader) ? equals((DefaultShader) obj) : false;
        }

        public bool equals(DefaultShader obj)
        {
            return (obj == this);
        }

        private Matrix3 normalMatrix = Matrix3.Identity;
        private float   time;
        private bool    lightsSet;

        public override void begin(Camera camera, RenderContext context)
        {
            base.begin(camera, context);

            foreach (DirectionalLight dirLight in directionalLights)
                dirLight.set(0, 0, 0, 0, -1, 0);
            foreach (PointLight pointLight in pointLights)
                pointLight.set(0, 0, 0, 0, 0, 0, 0);
            foreach (SpotLight spotLight in spotLights)
                spotLight.set(0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 1, 0);
            lightsSet = false;

            if (has(u_time)) set(u_time, time += Gdx.graphics.getDeltaTime());
        }

        public override void render(Renderable renderable, Attributes combinedAttributes)
        {
            if (!combinedAttributes.has(BlendingAttribute.Type))
                context.setBlending(false, BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            bindMaterial(combinedAttributes);
            if (lighting) bindLights(renderable, combinedAttributes);
            base.render(renderable, combinedAttributes);
        }

        public override void end()
        {
            base.end();
        }

        protected void bindMaterial(Attributes attributes)
        {
            var defaultCullFace  = CullFaceMode.Back;
            var defaultDepthFunc = DepthFunction.Lequal;

            var   cullFace       = config.defaultCullFace ?? defaultCullFace;
            var   depthFunc      = config.defaultDepthFunc ?? defaultDepthFunc;
            float depthRangeNear = 0f;
            float depthRangeFar  = 1f;
            bool  depthMask      = true;

            foreach (Attribute attr in attributes)
            {
                long t = attr.type;
                if (BlendingAttribute.@is(t))
                {
                    context.setBlending(true, ((BlendingAttribute) attr).sourceFunction, ((BlendingAttribute) attr).destFunction);
                    set(u_opacity, ((BlendingAttribute) attr).opacity);
                }
                else if ((t & IntAttribute.CullFace) == IntAttribute.CullFace)
                    cullFace = ((IntAttribute) attr).value;
                else if ((t & FloatAttribute.AlphaTest) == FloatAttribute.AlphaTest)
                    set(u_alphaTest, ((FloatAttribute) attr).value);
                else if ((t & DepthTestAttribute.Type) == DepthTestAttribute.Type)
                {
                    DepthTestAttribute dta = (DepthTestAttribute) attr;
                    depthFunc      = dta.depthFunc;
                    depthRangeNear = dta.depthRangeNear;
                    depthRangeFar  = dta.depthRangeFar;
                    depthMask      = dta.depthMask;
                }
                else if (!config.ignoreUnimplemented) throw new Exception("Unknown material attribute: " + attr.ToString());
            }

            context.setCullFace(cullFace);
            context.setDepthTest(depthFunc, depthRangeNear, depthRangeFar);
            context.setDepthMask(depthMask);
        }


        protected void bindLights(Renderable renderable, Attributes attributes)
        {
            var lights = renderable.environment;
            var dla    = attributes.get<DirectionalLightsAttribute>(DirectionalLightsAttribute.Type);
            var dirs   = dla?.lights;
            var pla    = attributes.get<PointLightsAttribute>(PointLightsAttribute.Type);
            var points = pla?.lights;
            var sla    = attributes.get<SpotLightsAttribute>(SpotLightsAttribute.Type);
            var spots  = sla?.lights;

            if (dirLightsLoc >= 0)
            {
                for (int i = 0; i < directionalLights.Length; i++)
                {
                    if (dirs == null || i >= dirs.Count)
                    {
                        if (lightsSet && directionalLights[i].color.r == 0f && directionalLights[i].color.g == 0f
                            && directionalLights[i].color.b == 0f) continue;
                        directionalLights[i].color = new Color(0, 0, 0, 1);
                    }
                    else if (lightsSet && directionalLights[i].equals(dirs[i]))
                        continue;
                    else
                        directionalLights[i].set(dirs[i]);

                    int idx = dirLightsLoc + i * dirLightsSize;
                    program.setUniformf(idx + dirLightsColorOffset, directionalLights[i].color.r, directionalLights[i].color.g,
                        directionalLights[i].color.b);
                    program.setUniformf(idx + dirLightsDirectionOffset, directionalLights[i].direction.X,
                        directionalLights[i].direction.Y, directionalLights[i].direction.Z);
                    if (dirLightsSize <= 0) break;
                }
            }

            if (pointLightsLoc >= 0)
            {
                for (int i = 0; i < pointLights.Length; i++)
                {
                    if (points == null || i >= points.Count)
                    {
                        if (lightsSet && pointLights[i].intensity == 0f) continue;
                        pointLights[i].intensity = 0f;
                    }
                    else if (lightsSet && pointLights[i].Equals(points[i]))
                        continue;
                    else
                        pointLights[i].set(points[i]);

                    int idx = pointLightsLoc + i * pointLightsSize;
                    program.setUniformf(idx + pointLightsColorOffset, pointLights[i].color.r * pointLights[i].intensity,
                        pointLights[i].color.g * pointLights[i].intensity, pointLights[i].color.b * pointLights[i].intensity);
                    program.setUniformf(idx + pointLightsPositionOffset, pointLights[i].position.X, pointLights[i].position.Y,
                        pointLights[i].position.Z);
                    if (pointLightsIntensityOffset >= 0) program.setUniformf(idx + pointLightsIntensityOffset, pointLights[i].intensity);
                    if (pointLightsSize <= 0) break;
                }
            }

            if (spotLightsLoc >= 0)
            {
                for (int i = 0; i < spotLights.Length; i++)
                {
                    if (spots == null || i >= spots.Count)
                    {
                        if (lightsSet && spotLights[i].intensity == 0f) continue;
                        spotLights[i].intensity = 0f;
                    }
                    else if (lightsSet && spotLights[i].Equals(spots[i]))
                        continue;
                    else
                        spotLights[i].set(spots[i]);

                    int idx = spotLightsLoc + i * spotLightsSize;
                    program.setUniformf(idx + spotLightsColorOffset, spotLights[i].color.r * spotLights[i].intensity,
                        spotLights[i].color.g * spotLights[i].intensity, spotLights[i].color.b * spotLights[i].intensity);
                    program.setUniformf(idx + spotLightsPositionOffset, spotLights[i].position);
                    program.setUniformf(idx + spotLightsDirectionOffset, spotLights[i].direction);
                    program.setUniformf(idx + spotLightsCutoffAngleOffset, spotLights[i].cutoffAngle);
                    program.setUniformf(idx + spotLightsExponentOffset, spotLights[i].exponent);
                    if (spotLightsIntensityOffset >= 0)
                        program.setUniformf(idx + spotLightsIntensityOffset, spotLights[i].intensity);
                    if (spotLightsSize <= 0) break;
                }
            }

            if (attributes.has(ColorAttribute.Fog))
            {
                set(u_fogColor, ((ColorAttribute) attributes.get(ColorAttribute.Fog)).color);
            }

            if (lights != null && lights.shadowMap != null)
            {
                set(u_shadowMapProjViewTrans, lights.shadowMap.getProjViewTrans());
                set(u_shadowTexture, lights.shadowMap.getDepthMap());
                set(u_shadowPCFOffset, 1.0f / (2f * lights.shadowMap.getDepthMap().texture.getWidth()));
            }

            lightsSet = true;
        }
    }
}