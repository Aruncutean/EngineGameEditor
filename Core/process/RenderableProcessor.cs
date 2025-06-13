using Core.component;
using Core.entity;
using Core.graphics;
using Core.graphics.light;
using Core.graphics.material;
using Core.graphics.material.texture;
using Core.graphics.shader;
using Core.scene;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Numerics;

namespace Core.process
{
    public class RenderableProcessor
    {
        public List<Entity> GetRenderables(World scene)
        {
            return scene.Entities
                .Where(e => e.HasComponent<MeshComponent>() &&
                            e.HasComponent<MaterialComponent>() &&
                            !e.HasComponent<LightComponent>())
                .ToList();
        }

        public void RenderEntity(Entity entity, List<Entity> lights, Matrix4x4 view, Matrix4x4 projection, GL gl, TransformComponent camera)
        {
            var transformComponent = entity.GetComponent<TransformComponent>();
            var meshComponent = entity.GetComponent<MeshComponent>();
            var materialComponent = entity.GetComponent<MaterialComponent>();

            if (transformComponent == null || meshComponent == null || materialComponent == null) return;

            var material = MaterialManager.Get(materialComponent.MaterialID);
            if (material == null) return;

            var shaderProgram = new ShaderProgram(ShaderManager.Get(material.Shader));
            shaderProgram.Use(gl);
            var binder = new ShaderBinder(gl, shaderProgram);

            Matrix4x4 model = Matrix4x4.CreateTranslation(transformComponent.Position);
            model = Matrix4x4.CreateRotationX(transformComponent.Rotation.X) * model;
            model = Matrix4x4.CreateRotationY(transformComponent.Rotation.Y) * model;
            model = Matrix4x4.CreateRotationZ(transformComponent.Rotation.Z) * model;
            model = Matrix4x4.CreateScale(transformComponent.Scale) * model;

            binder.SetMat4("model", model);
            binder.SetMat4("view", view);
            binder.SetMat4("projection", projection);

            if (material is MaterialDefault basic)
            {
                foreach (var light in lights)
                {
                    var lightComp = light.GetComponent<LightComponent>();
                    var lightTransform = light.GetComponent<TransformComponent>();
                    binder.SetVec3("lightPos", lightTransform.Position);
                    binder.SetVec3("viewPos", camera.Position);
                    binder.SetVec3("lightColor", new Vector3(1f, 1f, 1f));
                    binder.SetVec3("objectColor", new Vector3(1f, 0.5f, 0.3f));
                }

            }
            if (material is MaterialPhong phong)
            {

                binder.SetVec3("viewPos", camera.Position);
                binder.SetInt("material.ambientMap", 0);
                binder.SetInt("material.diffuseMap", 1);
                binder.SetInt("material.specularMap", 2);

                if (phong.Ambient.IsTexture == false)
                {
                    Console.WriteLine("material.ambientColor");
                    binder.SetVec3("material.ambientColor", ColorUtils.HexToVector3(phong.Ambient.Color));
                    binder.SetInt("material.useAmbientMap", 0);
                }
                else
                {
                    binder.SetInt("material.useAmbientMap", 1);
                    Console.WriteLine("material.useAmbientMap");
                    gl.ActiveTexture(TextureUnit.Texture0);
                    gl.BindTexture(TextureTarget.Texture2D, TextureManager.Get(phong.Ambient.TexturePath, gl));
                }

                if (phong.Diffuse.IsTexture == false)
                {
                    binder.SetVec3("material.diffuseColor", ColorUtils.HexToVector3(phong.Diffuse.Color));
                    binder.SetInt("material.useDiffuseMap", 0);
                }
                else
                {
                    binder.SetInt("material.useDiffuseMap", 1);

                    gl.ActiveTexture(TextureUnit.Texture1);
                    gl.BindTexture(TextureTarget.Texture2D, TextureManager.Get(phong.Diffuse.TexturePath, gl));
                }

                if (phong.Specular.IsTexture == false)
                {
                    binder.SetVec3("material.specularColor", ColorUtils.HexToVector3(phong.Specular.Color));
                    binder.SetInt("material.useSpecularMap", 0);
                }
                else
                {
                    binder.SetInt("material.useSpecularMap", 1);

                    gl.ActiveTexture(TextureUnit.Texture2);
                    gl.BindTexture(TextureTarget.Texture2D, TextureManager.Get(phong.Specular.TexturePath, gl));
                }

                binder.SetFloat("material.shininess", phong.Shininess);
                int i = 0;

                Console.WriteLine("Lights :" + lights.Count);
                foreach (var light in lights)
                {
                    var lightComp = light.GetComponent<LightComponent>();
                    var lightTransform = light.GetComponent<TransformComponent>();

                    Console.WriteLine("lightComp :" + lightComp.Type);

                    if (lightComp.LightBase is LightDirectional lightDirectional)
                    {
                        Console.WriteLine("dirLight :" + lightDirectional.Color);
                        Console.WriteLine("dirLight :" + lightDirectional.Intensity);
                        var direction = Vector3.Normalize(transformComponent.Position - lightTransform.Position);
                        binder.SetVec3("dirLight.direction", direction);
                        binder.SetVec3("dirLight.color", lightDirectional.Color);
                        binder.SetFloat("dirLight.intensity", lightDirectional.Intensity);
                    }

                    if (lightComp.LightBase is LightPoint lightPoint)
                    {
                        Attenuation.FromRange(lightPoint.Range, out float constant, out float linear, out float quadratic);
                        binder.SetVec3($"pointLights[{i}].position", lightTransform.Position);
                        binder.SetVec3($"pointLights[{i}].color", lightPoint.Color);
                        binder.SetFloat($"pointLights[{i}].intensity", lightPoint.Intensity);
                        binder.SetFloat($"pointLights[{i}].constant", constant);
                        binder.SetFloat($"pointLights[{i}].linear", linear);
                        binder.SetFloat($"pointLights[{i}].quadratic", linear);
                        i++;
                    }

                }
                binder.SetInt("numPointLights", i);
            }
            else if (material is MaterialPBR pbr)
            {

                foreach (var light in lights)
                {
                    var lightComp = light.GetComponent<LightComponent>();
                    var lightTransform = light.GetComponent<TransformComponent>();
                    binder.SetVec3("lightPos", lightTransform.Position);
                    binder.SetVec3("viewPos", camera.Position);
                    binder.SetVec3("lightColor", new Vector3(1f, 1f, 1f));
                    binder.SetVec3("objectColor", new Vector3(1f, 0.5f, 0.3f));
                }
            }

            if (meshComponent.gLMesh == null)
            {
                meshComponent.LoadMesh(gl);
            }

            meshComponent.gLMesh.Render();
        }
    }
}
