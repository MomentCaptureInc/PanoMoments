//using UnityEngine;
//using UnityEditor;
//using UnityEditor.Callbacks;
//using System.Collections;
//using System.IO;
//
//#if UNITY_IOS
//using UnityEditor.iOS.Xcode;
//using UnityEditor.iOS.Xcode.Extensions;
//
//public class BuildPostProcessor : MonoBehaviour {
//
//	[PostProcessBuildAttribute(1)]
//	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {
//
//		if (target != BuildTarget.iOS) {
//			UnityEngine.Debug.LogWarning ("Target is not iPhone. XCodePostProcess will not run");
//			return;
//		}
//#if UNITY_EDITOR_OSX
//		string projPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
//		PBXProject proj = new PBXProject();
//		proj.ReadFromString(File.ReadAllText(projPath));
//		string targetGuid = proj.TargetGuidByName("Unity-iPhone");
//		const string defaultLocationInProj = "Plugins/iOS";
//
//		// Framework 1
//		string coreFrameworkName = "PanoMoments.framework";
//		string framework = Path.Combine(defaultLocationInProj, coreFrameworkName);
//		string fileGuid = proj.AddFile(framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
//		PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
//		//		proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
//		//		proj.WriteToFile (projPath);
//
//		Debug.Log("projPath: " + projPath);
//		Debug.Log("defaultLocationInProj: " + defaultLocationInProj);
//		Debug.Log("framework: " + framework);
//		Debug.Log("fileGuid: " + fileGuid);
//
//		// Framework 2
//		coreFrameworkName = "PanoMomentsUnity.framework";
//		framework = Path.Combine(defaultLocationInProj, coreFrameworkName);
//		fileGuid = proj.AddFile(framework, "Frameworks/" + framework, PBXSourceTree.Sdk);
//		PBXProjectExtensions.AddFileToEmbedFrameworks(proj, targetGuid, fileGuid);
//
//		// Save
//		proj.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
//		proj.WriteToFile (projPath);
//#endif
//	}
//
//
//	//	static void ModifyFrameworksSettings(PBXProject project, string g) {
//	//
//	//		Debug.Log(">> Automation, Frameworks... <<");
//	//
//	//		project.AddFrameworkToProject(g, "PanoMoments.framework", false);
//	//		project.AddFrameworkToProject(g, "PanoMomentsUnity.framework", false);
//	//
//	//		Debug.Log(">> Automation, Settings... <<");
//
//	//		project.AddBuildProperty(g,
//	//			"LIBRARY_SEARCH_PATHS",
//	//			"../blahblah/lib");
//	//
//	//		project.AddBuildProperty(g,
//	//			"OTHER_LDFLAGS",
//	//			"-lsblah -lbz2");
//
//	// note that, due to some Apple shoddyness, you usually need to turn this off
//	// to allow the project to ARCHIVE correctly (ie, when sending to testflight):
//	//		project.AddBuildProperty(g,
//	//			"ENABLE_BITCODE",
//	//			"false");
//	//	}
//
//}
//#endif