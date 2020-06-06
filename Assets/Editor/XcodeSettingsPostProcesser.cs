using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;

public class XcodeSettingsPostProcesser
{

    [PostProcessBuild]
    public static void OnPostProcess(BuildTarget buildTarget, string buildPath)
    {
        // iOS以外のプラットフォームは処理を行わない
        if (buildTarget != BuildTarget.iOS)
            return;
        Debug.Log("OnPostProcess");

#if UNITY_IOS

        string projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");

        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        string targetGUID = proj.GetUnityFrameworkTargetGuid();
        proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-lz");
        proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-lsqlite3");
        proj.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-ObjC");

        var frameworks = new List<string>() {
            "UserNotifications.framework",
        };

        foreach (var framework in frameworks)
        {
            proj.AddFrameworkToProject(targetGUID, framework, false);
        }

        File.WriteAllText(projPath, proj.WriteToString());
#endif
    }
}
