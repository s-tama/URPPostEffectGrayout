using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GrayoutPass : ScriptableRenderPass
{
    const string Tag = "Grayout";
    const string ShaderNameGrayout = "Custom/Grayout";

    static readonly int ShaderPropertyIdGrayoutTmpRT = Shader.PropertyToID("_GrayoutTmpRT");
    static readonly int ShaderPropertyIdGrayoutPower = Shader.PropertyToID("_GrayoutPower");

    RenderTargetIdentifier _currentRenderTarget;
    Material _grayoutMaterial;
    float _grayoutPower;

    public GrayoutPass(RenderPassEvent evt)
    {
        base.renderPassEvent = evt;

        // グレーアウト用マテリアル生成
        if (_grayoutMaterial == null)
        {
            _grayoutMaterial = CoreUtils.CreateEngineMaterial(ShaderNameGrayout);
        }
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(Tag);
        using (new ProfilingScope(cmd, new ProfilingSampler(Tag)))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            Camera camera = renderingData.cameraData.camera;
            int w = camera.scaledPixelWidth;
            int h = camera.scaledPixelHeight;

            // 現在のレンダーターゲットをグレーアウト用レンダーターゲットにコピー
            cmd.GetTemporaryRT(ShaderPropertyIdGrayoutTmpRT, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
            cmd.Blit(_currentRenderTarget, ShaderPropertyIdGrayoutTmpRT);

            // グレーアウトフィルタをかけたのちレンダーターゲットにコピー
            _grayoutMaterial.SetFloat(ShaderPropertyIdGrayoutPower, _grayoutPower);
            cmd.Blit(ShaderPropertyIdGrayoutTmpRT, _currentRenderTarget, _grayoutMaterial);

            // 使用済みレンダーターゲット解放
            cmd.ReleaseTemporaryRT(ShaderPropertyIdGrayoutTmpRT);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void SetRenderTarget(RenderTargetIdentifier target)
    {
        _currentRenderTarget = target;
    }

    public void SetGrayoutPower(float value)
    {
        _grayoutPower = value;
    }
}
