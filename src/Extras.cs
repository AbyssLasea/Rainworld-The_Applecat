using System;
using System.Security.Permissions;
using UnityEngine;

/*
 * ���ļ��������޸�������ʱһЩ����������޸���
 * ������֪���Լ�����ʲô�������㲻Ӧ���������޸��κ����ݡ�
 */

// �������˽�˳�Ա
#pragma warning disable CS0618
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618


internal static class Extras
{
    private static bool _initialized;

    // ȷ����Դֻ����һ�Σ����Ҽ���ʧ�ܲ����ƻ�����ģ��
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