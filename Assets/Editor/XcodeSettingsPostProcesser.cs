using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.iOS.Xcode;
using UnityEditor.Callbacks;
using System.Collections;
using System.Collections.Generic;

public class XcodeSettingsPostProcesser : ScriptableObject
{

    [PostProcessBuild]
    public static void OnPostProcess(BuildTarget buildTarget, string buildPath)
    {
        // iOS以外のプラットフォームは処理を行わない
        if (buildTarget != BuildTarget.iOS)
            return;


#if UNITY_IOS

        string projPath = Path.Combine(buildPath, "Unity-iPhone.xcodeproj/project.pbxproj");

        PBXProject proj = new PBXProject();
        proj.ReadFromString(File.ReadAllText(projPath));

        string target = proj.TargetGuidByName("Unity-iPhone");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-lz");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-lsqlite3");
        proj.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

        var frameworks = new List<string>() {
            "UserNotifications.framework",
        };

        foreach (var framework in frameworks)
        {
            proj.AddFrameworkToProject(target, framework, false);
        }

        File.WriteAllText(projPath, proj.WriteToString());
#endif
    }
}
