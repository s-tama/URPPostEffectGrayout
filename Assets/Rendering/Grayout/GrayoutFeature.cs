using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GrayoutFeature : ScriptableRendererFeature
{
    public const int RenderingLayerMaskGrayout = 1;

    [SerializeField, Range(0f, 1f)] float _grayoutPower = 0.5f;
    GrayoutPass _grayoutPass;
    DrawObjectsIgnoreGrayoutPass _ignoreGrayoutPass;

    public override void Create()
    {
        _grayoutPass = new GrayoutPass(RenderPassEvent.AfterRenderingSkybox);
        _ignoreGrayoutPass = new DrawObjectsIgnoreGrayoutPass(RenderPassEvent.AfterRenderingSkybox);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        _grayoutPass.SetRenderTarget(renderer.cameraColorTarget);
        _grayoutPass.SetGrayoutPower(_grayoutPower);
        renderer.EnqueuePass(_grayoutPass);
        renderer.EnqueuePass(_ignoreGrayoutPass);
    }
}
