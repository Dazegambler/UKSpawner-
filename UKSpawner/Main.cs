using System;
using System.Collections;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
/// <summary>
/// Todo:
/// 2-FIX V2.2 NAILGUN
/// ???-Cleanup Code
/// </summary>
namespace UKSpawner
{
    [BepInPlugin("UK.Spawner","UKSpawner","1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<KeyCode> Spawn,Menu,Arm;

        static AssetBundle UIBundle = Plugin.LoadAssetBundle(Properties.Resources.ukspawner);
        GUISkin skin = UIBundle.LoadAsset<GUISkin>("UISpawner");
        float sWidth = Screen.width, ratio;
        Vector3 _ratio;

        int id = 0;
        bool menu,ready, greed;
        GameObject[] Check,Enemies;
        string Name = "MinosPrime";
        GameObject Trgt,Cam,Hpbar,Player;

        GameObject nail;

        string Title;
        Rect wind,SpawnMenu, AttacksMenu;

        Scene ACT;

        AssistController asscont;
        CameraController Cc;
        public void Start()
        {
            Spawn = Config.Bind("Binds", "Spawn Enemy",KeyCode.T,"");
            Arm = Config.Bind("Binds", "Arm Spawner",KeyCode.U,"");
            Menu = Config.Bind("Binds", "Open Spawn Menu", KeyCode.Y, "");
            //UIBundle.Unload(false);
        }
        public void Update()
        {
            switch (ACT.name)
            {
                case "Intro":
                    break;
                case "Main Menu":
                    break;
                default:
                    if (Time.timeSinceLevelLoad < .2f) Reset();
                    Enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    StartCoroutine("SlowUpdate");
                    break;
            }
        }
        void Reset()
        {
            Check = Resources.FindObjectsOfTypeAll<GameObject>();
            ready = false;
            menu = false;
            Player = GameObjFind("Player", Player);
            Cam = GameObjFind("Main Camera", Cam);
            Cc = GameObject.Find("Main Camera").GetComponent<CameraController>();
            asscont = GameObject.Find("GameController").GetComponent<AssistController>();
            Hpbar = GameObject.Find("Canvas/Boss Healths");
            trgt("MinosPrime");

        }
        public void OnGUI()
        {
            //GUI.matrix = Matrix4x4.TRS(new Vector3(_ratio.x, _ratio.y, 0), Quaternion.identity, _ratio);
            if (GUI.skin != skin)
            {
                GUI.skin = skin;
            }
            switch (ACT.name)
            {
                case "Intro":
                    break;
                case "Main Menu":
                    break;
                default:
                    switch (id)
                    {
                        case 1:
                            if(asscont.majorEnabled != true || asscont.cheatsEnabled != true)
                            {
                                StartCoroutine("Warning");
                                id = 0;
                            }
                            break;
                    }
                    switch (menu)
                    {
                        case true:
                            GUI.Window(id,wind,UI,Title);
                            switch (id)
                            {
                                case 0:
                                    wind = SpawnMenu;
                                    Title = "Enemies Menu";
                                    break;
                                case 1:
                                    wind = AttacksMenu;
                                    Title = "Projectiles Menu";
                                    break;
                            }
                            switch (ready)
                            {
                                case true:
                                    Text("Spawner Ready", new Rect(7f, 0f, 2000f, 80f),25,"blue");
                                    switch (Trgt)
                                    {
                                        case null:
                                            break;
                                        default:
                                            switch (Trgt.name)
                                            {
                                                case "V2":
                                                    Text(Trgt.name, new Rect(7f, 25f, 2000f, 80f), 25, "red");
                                                    Text("Greed: " + greed, new Rect(7f, 50f, 2000f, 80f), 25, "maroon");
                                                    break;
                                                default:
                                                    Text(Trgt.name, new Rect(7f, 25f, 2000f, 80f), 25, "red");
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        public void UI(int WindowID)
        {
            switch (WindowID)
            {
                case 0:
                    EnemiesMnu();
                    break;
                case 1:
                    ProjMnu();
                    break;
            }
        }
        void EnemiesMnu()
        {
            Tab("<size=13>ENEMIES</size>", new Rect(115, 20, 75, 25), 0);
            Tab("<size=13>PROJECTILES</size>", new Rect(5, 20, 100, 25), 1);
            Opt("MinosPrime", new Rect(5, 50, 200, 30));
            Opt("V2", new Rect(5, 90, 200, 30));
            if (GUI.Button(new Rect(215, 90, 75, 30), "Greed"))
            {
                greed = !greed;
            }
            Opt("Gabriel", new Rect(5, 130, 200, 30));
            Opt("DroneFlesh", new Rect(5, 170, 200, 30));
            Opt("Flesh Prison", new Rect(5, 210, 200, 30));
            Opt("MinosBoss", new Rect(5, 250, 200, 30));
            Opt("Wicked", new Rect(5, 290, 200, 30));
            Opt("DroneSkull Variant", new Rect(5, 330, 200, 30));
        }
        void ProjMnu()
        {
            Tab("<size=13>ENEMIES</size>", new Rect(115, 20, 75, 25), 0);
            Tab("<size=13>PROJECTILES</size>", new Rect(5, 20, 100, 25), 1);
            Opt("VirtueInsignia", new Rect(5, 50, 200, 30));
            Opt("MindflayerBeam", new Rect(5, 90, 200, 30));
            Opt("Projectile", new Rect(5, 130, 200, 30));
            Opt("ProjectileSpread", new Rect(5, 170, 200, 30));
        }
        private void Text(string txt,Rect Pos,int fntsize,string col)
        {
            //Generates Funky text with custom colors
            GUI.Label(new Rect(Pos.x + 1f, Pos.y - 1f, Pos.width, Pos.height), $"<size={fntsize}><color=black>{txt}</color></size>");
            GUI.Label(Pos, $"<size={fntsize}><color={col}>{txt}</color></size>");
        }
        IEnumerator Warning()
        {
            Text("<b>MUST HAVE MAJOR ASSISTS/CHEATS ACTIVATED</b>", new Rect(Screen.width / 4, Screen.height / 2, 1000, 100),40,"red");
            yield return new WaitForSecondsRealtime(5);
        }
        private void Opt(string txt,Rect pos)
        {
            //Generate New Enemy Option
            switch(GUI.Button(pos, txt))
            {
                case true:
                    Name = txt;
                    break;
            }
        }
        private void Tab(string txt,Rect pos,int num)
        {
            //Generates new Tab button
            switch (GUI.Button(pos, txt))
            {
                case true:
                    id = num;
                    break;
            }
        }
        private void Inst(GameObject obj)
        {
            //Spawns Selected Enemy
            switch (obj.name)
            {
                case "MindflayerBeam":
                    RaycastHit endpos;
                    switch (Physics.Raycast(Cc.transform.position, Cc.transform.forward, out endpos, 200f))
                    {
                        case true:
                            Instantiate(Trgt, new Vector3(Cc.transform.position.x - 3, Cc.transform.position.y + 1, Cc.transform.position.z), Cc.transform.rotation);
                            break;
                    }
                    break;
                case "Projectile":
                    Instantiate(Trgt, new Vector3(Cc.transform.position.x, Cc.transform.position.y - 1, Cc.transform.position.z + 2.5f), Cc.transform.rotation);
                    break;
                case "ProjectileSpread":
                    Instantiate(Trgt, new Vector3(Cc.transform.position.x, Cc.transform.position.y - 1, Cc.transform.position.z + 2.5f), Cc.transform.rotation);
                    break;
                case "V2":
                    switch (greed)
                    {
                        case true:
                            V2dif(true);
                            break;
                        case false:
                            V2dif(false);
                            break;
                    }
                    RaycastHit pos;
                    switch (Physics.Raycast(Cc.transform.position, Cc.transform.forward, out pos, 200f))
                    {
                        case true:
                            Instantiate(Trgt, pos.point, Quaternion.identity);
                            break;
                    }
                    break;
                default:
                    RaycastHit _Spawnpos;
                    switch (Trgt.activeSelf)
                    {
                        case false:
                            Trgt.SetActive(true);
                            break;
                    }
                    switch (Physics.Raycast(Cc.transform.position, Cc.transform.forward, out _Spawnpos, 200f))
                    {
                        case true:
                            Instantiate(Trgt, _Spawnpos.point, Quaternion.identity);
                            break;
                    }
                    break;
            }
        }
        private void TrgtSetup()
        {
            //Finds Enemy in the Existing Assets
            switch (Check)
            {
                case null:
                    break;
                default:
                    switch (ready)
                    {
                        case true:
                            switch (Trgt)
                            {
                                case null:
                                    trgt(Name);
                                    break;
                                default:
                                    if (Trgt.name != Name)
                                    {
                                        trgt(Name);
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }
        IEnumerator SlowUpdate()
        {
            V2Destroy();
            TrgtSetup();
            SmolHealth();
            StartCoroutine("Findobjs");
            //ProjHoming();
            switch (Player)
            {
                case null:
                    Player = GameObjFind("Player", Player);
                    break;
            }
            switch (Cam)
            {
                case null:
                    Cam = GameObjFind("Main Camera", Cam);
                    break;
                default:
                    switch (Cc)
                    {
                        case null:
                            Cc = Cam.GetComponent<CameraController>();
                            break;
                    }
                    break;
            }
            switch (asscont)
            {
                case null:
                    asscont = GameObject.Find("GameController").GetComponent<AssistController>();
                    break;
            }
            switch (Input.GetKeyDown(Arm.Value))
            {
                case true:
                    ready = !ready;
                    break;
            }
            switch (ready)
            {
                case true:
                    switch (Input.GetKeyDown(Menu.Value))
                    {
                        case true:
                            menu = !menu;
                            break;
                    }
                    switch (Input.GetKeyDown(Spawn.Value))
                    {
                        case true:
                            Inst(Trgt);
                            break;
                    }
                    break;
            }
            yield return new WaitForSecondsRealtime(1f);
        }
        IEnumerator Findobjs()
        {
            if (ratio != sWidth / 1920)
            {
                ratio = sWidth / 1920;
            }
            if (SpawnMenu != new Rect(ratio + 60, ratio + 400, 300, 370))
            {
                SpawnMenu = new Rect(ratio + 60, ratio + 400, 300, 370);
            }
            if (AttacksMenu != new Rect(ratio + 60, ratio + 400, 210, 205))
            {
                AttacksMenu = new Rect(ratio + 60, ratio + 400, 210, 205);
            }
            if (_ratio != new Vector3(ratio, ratio, 1))
            {
                _ratio = new Vector3(ratio, ratio, 1);
            }
            ACT = SceneManager.GetActiveScene();
            Check = Resources.FindObjectsOfTypeAll<GameObject>();
            yield return new WaitForSecondsRealtime(1f);
        }
        private void trgt(string txt)
        {
            //Chooses your selected Enemy
            foreach (GameObject Obj in Check)
            {
                if (Obj.gameObject.name == txt)
                {
                    switch (txt)
                    {
                        case "V2":
                            switch (Obj.tag)
                            {
                                case "Enemy":
                                    Trgt = Obj;
                                    break;
                            }
                            break;
                        default:
                            Trgt = Obj;
                            break;
                    }
                }
            }
        }
        private void V2Destroy()
        {
            //V2 is a bitch when it dies so this code just makes it Immedietly be destroyed  on Death
            switch (Enemies)
            {
                case null:
                    break;
                default:
                    foreach (GameObject Enmy in Enemies)
                    {
                        switch (Enmy.name)
                        {
                            case "V2(Clone)":
                                var eid = Enmy.GetComponent<EnemyIdentifier>();
                                switch (eid.dead)
                                {
                                    case true:
                                        Destroy(Enmy);
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    break;
            }
        }
        private void V2dif(bool greed)
        {
            //V2 likes being special so this will set if greed V2 or limbo V2
            var v2 = Trgt.GetComponent<V2>();
            var mch = Trgt.GetComponent<Machine>();
            List<GameObject> weps = new List<GameObject>(v2.weapons);

            switch (nail)
            {
                case null:
                    foreach (Transform obj in v2.gameObject.GetComponentsInChildren<Transform>(true))
                    {
                        switch (obj.name)
                        {
                            case "Nailgun":
                                nail = obj.gameObject;
                                break;
                        }
                    }
                    break;
            }
            switch (greed)
            {
                case true:
                    v2.secondEncounter = true;
                    mch.health = 80f;
                    nail.SetActive(true);
                    switch (weps.Contains(nail))
                    {
                        case false:
                            weps.Add(nail);
                            v2.weapons = weps.ToArray();
                            break;
                    }
                    break;
                default:
                    v2.secondEncounter = false;
                    mch.health = 40f;
                    nail.SetActive(false);
                    switch (weps.Contains(nail))
                    {
                        case true:
                            weps.Remove(nail);
                            v2.weapons = weps.ToArray();
                            break;
                    }
                    break;
            }
        }
        private GameObject AssetFind(string name,GameObject Original)
        {
            //Find set Object in the prefabs
            foreach (GameObject obj in Check)
            {
                if(obj.gameObject.name == name)
                {
                    Original = obj;
                }
            }
            return Original;
        }
        private GameObject GameObjFind(string name,GameObject target)
        {
            //Find Object in Scene
            switch (target)
            {
                case null:
                    target = GameObject.Find(name);
                    break;
            }
            return target;
        }
        private void SmolHealth()
        {
            //updates health bars to be less huge
            switch (Hpbar)
            {
                case null:
                    break;
                default:
                    try
                    {
                        if (Hpbar.transform.localPosition != new Vector3(483f, -267, 0))
                        {
                            Hpbar.transform.localPosition = new Vector3(483f, -267, 0);
                        }
                        if (Hpbar.transform.localScale != new Vector3(0.25f, 0.25f, 0.25f))
                        {
                            Hpbar.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Debug.LogWarning($"Object {Hpbar.name} NOT FOUND...ATTEMPTING TO FIND OBJECT");
                        Hpbar = GameObject.Find("Canvas/Boss Healths");
                    }
                    break;
            }
        }
        private void ProjHoming()
        {
        }
        static AssetBundle LoadAssetBundle(byte[] Bytes)
        {
            if (Bytes == null) throw new ArgumentNullException(nameof(Bytes));
            var bundle = AssetBundle.LoadFromMemory(Bytes);
            return bundle;
        }
    }
}
