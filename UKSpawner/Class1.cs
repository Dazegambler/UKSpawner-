using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace UKSpawner
{
    [BepInPlugin("UK.Spawner","UKSpawner","1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<KeyCode> Spawn,Menu,Arm;
        private ConfigEntry<Color> Main, Back;

        bool menu= false,armed = false;
        GameObject[] Check = null;
        string Name = "MinosPrime";
        GameObject Trgt,Cam;
        public Rect SpawnMenu = new Rect(60, 400, 400, 200);

        public void Start()
        {
            Spawn = Config.Bind("Binds", "Spawn Enemy",KeyCode.O,"");
            Arm = Config.Bind("Binds", "Arm Spawner",KeyCode.U,"");
            Menu = Config.Bind("Binds", "Open Spawn Menu", KeyCode.L, "");
            Main = Config.Bind("Colors", "Main Color", Color.white, "");
            Back = Config.Bind("Colors", "Background Color", Color.blue, "");
        }
        public void Update()
        {
            TrgtSetup();
            if (Cam == null)
            {
                Cam = GameObject.Find("Main Camera");
            }
            if (Input.GetKeyDown(Arm.Value))
            {
                armed = !armed;
            }
            if (armed)
            {
                if (Input.GetKeyDown(Menu.Value))
                {
                    menu = !menu;
                }
                if (Input.GetKeyDown(Spawn.Value))
                {
                    Inst();
                }
            }
        }
        private void OnGUI()
        {
            var STY = new GUIStyle();
            STY.fontSize = 25;
            STY.normal.textColor = Main.Value;
            var STYB = new GUIStyle();
            STYB.fontSize = 26;
            STYB.normal.textColor = Back.Value;
            if (menu)
            {
                SpawnMenu = GUI.Window(0, SpawnMenu, UI, "Spawn Menu", STY);
            }
            if (armed)
            {
                Text("Spawner Ready", new Rect(7f, 0f, 200f, 20f), STY, STYB);
                if (Trgt != null)
                {
                    Text(Trgt.name, new Rect(7f, 25f, 200f, 20f), STY, STYB);
                }
            }
        }
        public void UI(int WindowID)
        {
            switch (WindowID)
            {
                case 0:
                    Opt("MinosPrime", new Rect(5, 30, 200, 20));
                    Opt("V2", new Rect(5, 50, 200, 20));
                    Opt("Gabriel", new Rect(5, 70, 200, 20));
                    Opt("DroneFlesh", new Rect(5, 90, 200, 20));
                    Opt("Flesh Prison", new Rect(5, 110, 200, 20));
                    Opt("MinosBoss", new Rect(5, 130, 200, 20));
                    Opt("Wicked", new Rect(5, 150, 200, 20));
                    Opt("DroneSkull Variant", new Rect(5, 170, 200, 20));
                    break;
            }
        }
        private void Text(string txt,Rect pos,GUIStyle main,GUIStyle back)
        {
            GUI.Label(new Rect(pos.x-2,pos.y,pos.width,pos.height), txt, back);
            GUI.Label(pos, txt, main);
        }
        private void Opt(string txt,Rect pos)
        {
            if (GUI.Button(pos, txt))
            {
                Name = txt;
            }
        }
        private void Inst()
        {
            var Cc = Cam.GetComponent<CameraController>();
            RaycastHit Spawnpos;
            if(Physics.Raycast(Cc.transform.position,Cc.transform.forward,out Spawnpos, 200f))
            {
                Instantiate(Trgt, Spawnpos.point, Quaternion.identity);
                Trgt.SetActive(true);
            }
        }
        private void TrgtSetup()
        {
            if(Check == null)
            {
                Check = Resources.FindObjectsOfTypeAll<GameObject>();
            }
            if (Check != null && Trgt == null && armed)
            {
                trgt(Name);
            }
            else if (Check != null && Trgt != null && armed)
            {
                if(Trgt.name != Name)
                {
                    trgt(Name);
                }
            }
        }
        private void trgt(string txt)
        {
            foreach (GameObject Obj in Check)
            {
                if (Obj.gameObject.name == txt)
                {
                    Trgt = Obj;
                }
            }
        }
    }
}
