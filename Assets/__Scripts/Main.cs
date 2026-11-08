using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static private Main S;
    static private Dictionary<eWeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Inscribed")]
    public bool spawnEnemies = true;
    public GameObject[] prefabEnemies;
    public float enemySpawnPerSecond = 0.5f;
    public float enemyInsetDefault = 1.5f;
    public float gameRestartDelay = 2;
    public GameObject prefabPowerUp;
    public WeaponDefinition[] weaponDefinitions;
    public eWeaponType[] powerUpFrequency = new eWeaponType[]{eWeaponType.blaster, eWeaponType.blaster, eWeaponType.spread, eWeaponType.shield};
    private BoundsCheck bndCheck;

    public GameObject heroPrefab;
    public List<GameObject> lives;
    public int livesCount = 3;
    public GameObject livesPrefab;
    public int points = 0;
    public TextMesh keptScore;
    

    void Start()
    {
        GameObject obj = GameObject.Find("Score");
        keptScore = obj.GetComponent<TextMesh>();
        lives = new List<GameObject>();
        for (int i = 0; i < livesCount; i++)
        {
            GameObject livesGO = Instantiate<GameObject>(livesPrefab);
            Vector3 pos = livesGO.transform.position;
            pos.x -= 3 * i;
            livesGO.transform.position = pos;
            lives.Add(livesGO);
        }
    }

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);

        WEAP_DICT = new Dictionary<eWeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        if (!spawnEnemies)
        {
            Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
            return;
        }
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        float enemyInset = enemyInsetDefault;
        if(go.GetComponent<BoundsCheck>() != null)
        {
            enemyInset = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        Vector3 pos = Vector3.zero;
        float xMin = -bndCheck.camWidth + enemyInset;
        float xMax = bndCheck.camWidth - enemyInset;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndCheck.camHeight + enemyInset;

        go.transform.position = pos;

        Invoke(nameof(SpawnEnemy), 1f/enemySpawnPerSecond);
    }

    void DelayedRestart()
    {
        Invoke(nameof(Restart), gameRestartDelay);
    }

    void Restart()
    {
        SceneManager.LoadScene("__Scene_0");
    }

    static public void HERO_DIED()
    {
        if(S.lives.Count != 0)
        {
            int liveIndex = S.lives.Count - 1;
            GameObject temp = S.lives[liveIndex];
            S.lives.RemoveAt(liveIndex);
            Destroy(temp);

        }
        else
        {
            S.DelayedRestart();
            Destroy(Hero.S);
        }
        //S.DelayedRestart();
    }

    static public WeaponDefinition GET_WEAPONDEFINITION(eWeaponType wt)
    {
        if (WEAP_DICT.ContainsKey(wt))
        {
            return(WEAP_DICT[wt]);
        }
        return(new WeaponDefinition());
    }

    static public void SHIP_DESCTROYED(Enemy e)
    {
        if (Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, S.powerUpFrequency.Length);
            eWeaponType pUpType = S.powerUpFrequency[ndx];
            
            GameObject go = Instantiate<GameObject>(S.prefabPowerUp);
            PowerUp pUp = go.GetComponent<PowerUp>();
            pUp.SetType(pUpType);

            pUp.transform.position = e.transform.position;
            
        }
        S.points += e.score;
        S.keptScore.text = "Score: " + S.points;
    }
}
