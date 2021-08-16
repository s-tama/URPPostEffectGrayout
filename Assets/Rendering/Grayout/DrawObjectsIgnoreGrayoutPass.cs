using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DrawObjectsIgnoreGrayoutPass : ScriptableRenderPass
{
    const string Tag = "IgnoreGrayout";

    static readonly ShaderTagId ShaderTagIdUniversalForward = new ShaderTagId("IgnoreGrayout");

    FilteringSettings _ignoreGrayoutFilter;

    public DrawObjectsIgnoreGrayoutPass(RenderPassEvent evt)
    {
        // �O���[�A�E�g��K�p���Ȃ��I�u�W�F�N�g�̃t�B���^�����O
        uint ignoreGrayoutMask = uint.MaxValue ^ GrayoutFeature.RenderingLayerMaskGrayout;
        _ignoreGrayoutFilter = new FilteringSettings(RenderQueueRange.opaque, -1, ignoreGrayoutMask);
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        CommandBuffer cmd = CommandBufferPool.Get(Tag);
        using (new ProfilingScope(cmd, new ProfilingSampler(Tag)))
        {
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;

            // �O���[�A�E�g��K�p���Ȃ��I�u�W�F�N�g�`��
            DrawingSettings drawingSettings = CreateDrawingSettings(ShaderTagIdUniversalForward, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = null;
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _ignoreGrayoutFilter);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }
}
