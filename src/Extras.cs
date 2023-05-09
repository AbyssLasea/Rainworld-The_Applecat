using System;
using System.Security.Permissions;
using UnityEngine;

/*
 * 此文件包含对修改雨世界时一些常见问题的修复。
 * 除非你知道自己在做什么，否则你不应该在这里修改任何内容。
 */

// 允许访问私人成员
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618


internal static class Extras
{
    private static bool _initialized;

    // 确保资源只加载一次，并且加载失败不会破坏其他模组
    public static On.RainWorld.hook_OnModsInit WrapInit(Action<RainWorld> loadResources)
    {
        return (orig, self) =>
        {
            orig(self);

            try
            {
                if (!_initialized)
                {
                    _initialized = true;
                    loadResources(self);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        };
    }
}