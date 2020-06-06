using MelonLoader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VRC;
using VRC.Core;

[assembly: MelonModInfo(typeof(PHorseClOnE.Main), "PhoreClOnE","1.0","404#0004")]
[assembly: MelonModGame("VRChat", "VRChat")]
namespace PHorseClOnE
{
    public class Main : MelonMod 
    {
        public static GameObject OldClone = null;
        public static Button OldBtn = null;
        public static GameObject CloneTrnf = null;
        public static Button CloneBtn = null;
        public static Text CloneTxt = null;
        public static MethodInfo AvatarSwitch;
        public static MethodInfo apiapublic;

        public static void ForceSwitch(ApiAvatar avi)
        {
            if (apiapublic == null || AvatarSwitch == null)
            {
                var type = AppDomain.CurrentDomain.GetAssemblies().First(x => x.FullName.Contains("Assembly-CSharp,")).GetTypes().First(x => x.BaseType != null && x.BaseType == typeof(APIUser));
                apiapublic = type.GetProperties().First(x => x.PropertyType == type).GetGetMethod();
                AvatarSwitch = type.GetMethod("Method_Public_Void_ApiAvatar_String_0");
            }
            AvatarSwitch.Invoke(apiapublic.Invoke(null, null), new object[] { avi, "AvatarMenu" });
        }

        public override void OnUpdate()
        {
            if (RoomManagerBase.field_Internal_Static_ApiWorld_0 == null && RoomManagerBase.field_Internal_Static_ApiWorldInstance_0 == null)
                return;

            if (CloneTrnf != null && CloneTrnf.gameObject.activeInHierarchy)
            {
                CloneBtn.colors = OldBtn.colors;
                CloneTrnf.transform.position = OldClone.transform.position;

                if (QuickMenu.prop_QuickMenu_0.prop_APIUser_0 != null)
                {
                    if (!QuickMenu.prop_QuickMenu_0.prop_APIUser_0.allowAvatarCopying)
                    {
                        CloneTxt.text = "Force-Clone";
                    }
                    else
                    {
                        CloneTxt.text = "Clone";
                    }
                }

            }
        }
        public override void VRChat_OnUiManagerInit()
        {
            OldClone = QuickMenu.prop_QuickMenu_0.transform.Find("UserInteractMenu/CloneAvatarButton").gameObject; //Ez fucking clap
            OldBtn = OldClone.GetComponentInChildren<Button>();
            CloneTrnf = UnityEngine.Object.Instantiate(OldClone, OldClone.transform.parent);
            OldClone.transform.localScale = Vector3.zero;
            CloneBtn = CloneTrnf.GetComponentInChildren<Button>();
            CloneTxt = CloneTrnf.GetComponentInChildren<Text>();
            CloneBtn.onClick = new Button.ButtonClickedEvent();
            CloneTxt.text = "";
            CloneBtn.onClick.AddListener(new Action(() => {
                if (QuickMenu.prop_QuickMenu_0.prop_APIUser_0 != null)
                {
                    var theplayer = PlayerManager.Method_Public_Static_Player_String_1(QuickMenu.prop_QuickMenu_0.prop_APIUser_0.id);
                    if (theplayer != null && theplayer.prop_VRCAvatarManager_0 != null && theplayer.prop_VRCAvatarManager_0.prop_ApiAvatar_0 != null)
                    {
                        var avatar = theplayer.prop_VRCAvatarManager_0.prop_ApiAvatar_0;
                        MelonModLogger.Log(avatar.releaseStatus);
                        if (avatar.releaseStatus == "public")
                        {
                            ForceSwitch(theplayer.prop_VRCAvatarManager_0.prop_ApiAvatar_0);
                        } else
                        {
                            VRCUiPopupManager.prop_VRCUiPopupManager_0.Method_Public_Void_String_String_Single_0("This is private.","Rip it instead.",3f);
                        }
                    }
                }
            }));

        }

    }
}
