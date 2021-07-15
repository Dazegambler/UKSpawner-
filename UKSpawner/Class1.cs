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
    [BepInPlugin("UK.Spawner","UKSpawner","1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<KeyCode> Spawn,Menu,Arm;
        private ConfigEntry<Color> Main, Back;

        bool menu= false,armed = false,greed = false;
        GameObject[] Check = null, V2Check = null;
        string Name = "MinosPrime";
        GameObject Trgt,Cam;
        public Rect SpawnMenu = new Rect(60, 400, 600, 400);

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
            V2check();
            TrgtSetup();
            SmolHealth();
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
                    if(Trgt.name == "V2")
                    {
                        switch (greed)
                        {
                            case true:
                                V2dif(true);
                                break;
                            case false:
                                V2dif(false);
                                break;
                        }
                        Inst();
                    }
                    else
                    {
                        Inst();
                    }
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
                    if(Trgt.name == "V2")
                    {
                        Text(Trgt.name, new Rect(7f, 25f, 200f, 20f), STY, STYB);
                        Text("Greed: " + greed, new Rect(7f, 50f, 200f, 20f), STY, STYB);
                    }
                    else
                    {
                        Text(Trgt.name, new Rect(7f, 25f, 200f, 20f), STY, STYB);
                    }
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
                    if(GUI.Button(new Rect(205, 50, 100, 20), "Greed"))
                    {
                        greed = !greed;
                    }
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
            //Generates Funky text with custom colors
            GUI.Label(new Rect(pos.x-2,pos.y,pos.width,pos.height), txt, back);
            GUI.Label(pos, txt, main);
        }
        private void Opt(string txt,Rect pos)
        {
            //Generate New Enemy Option
            if (GUI.Button(pos, txt))
            {
                Name = txt;
            }
        }
        private void Inst()
        {
            //Spawns Selected Enemy
            RaycastHit Spawnpos;
            var Cc = Cam.GetComponent<CameraController>();
            if (Physics.Raycast(Cc.transform.position, Cc.transform.forward, out Spawnpos, 200f)) 
            {
                Instantiate(Trgt, Spawnpos.point, Quaternion.identity);
                Trgt.SetActive(true);
            }
        }
        private void TrgtSetup()
        {
            //Finds Enemy in the Existing Assets
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
            //Chooses your selected Enemy
            foreach (GameObject Obj in Check)
            {
                if (Obj.gameObject.name == txt)
                {
                    if(txt == "V2")
                    {
                        if(Obj.tag == "Enemy")
                        {
                            Trgt = Obj;
                        }
                        else
                        {
                        }
                    }
                    else
                    {
                        Trgt = Obj;
                    }
                }
            }
        }
        private void V2check()
        {
            //V2 is a bitch when it dies so this code just makes it Immedietly Disappear on Death
            V2Check = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject Enmy in V2Check)
            {
                if (Enmy.gameObject.name == "V2(Clone)")
                {
                    var mch = Enmy.GetComponent<Machine>();
                    var nail = Enmy.GetComponentInChildren<EnemyNailgun>();
                    var v2 = Enmy.GetComponent<V2>();
                    if(mch.health <= 0)
                    {
                        Destroy(Enmy);
                    }
                    if (v2.secondEncounter == true)
                    {
                    }
                }
            }
        }
        private void V2dif(bool greed)
        {
            //V2 likes being special so this will set if greed V2 or limbo V2
            var v2 = Trgt.GetComponent<V2>();
            var mch = Trgt.GetComponent<Machine>();
            if (greed)
            {
                v2.secondEncounter = true;
                mch.health = 80f;
            }
            else
            {
                v2.secondEncounter = false;
                mch.health = 40f;
            }
        }
        private GameObject ObjFind(string name,GameObject Original)
        {
            GameObject[] pool = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (GameObject obj in pool)
            {
                if(obj.gameObject.name == name)
                {
                    Original = obj;
                }
            }
            return Original;
        }
        private void SmolHealth()
        {
            GameObject Hpbar = GameObject.Find("Boss Healths");
            Hpbar.transform.localPosition = new Vector3(483f,-267,0);
            Hpbar.transform.localScale = new Vector3(0.25f,0.25f,0.25f);
        }
    }
}
