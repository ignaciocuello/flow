using System.Collections.Generic;
using UnityEngine;

public class EntityFactory : Singleton<EntityFactory>  {

    [SerializeField]
    private GameObject[] prefabs;

    private Dictionary<string, GameObject> folders;
    private Dictionary<string, GameObject> prefabDict;

    protected override void Initialize()
    {
        folders = new Dictionary<string, GameObject>();
        prefabDict = new Dictionary<string, GameObject>();

        for (int i = 0; i < transform.childCount; i++)
        {
            folders.Add(transform.GetChild(i).name, transform.GetChild(i).gameObject);
        }

        foreach (GameObject prefab in prefabs)
        {
            string pluralName = ToPlural(prefab.name);
            if (!folders.ContainsKey(pluralName))
            {
                GameObject folder = new GameObject
                {
                    name = ToPlural(prefab.name)
                };
                folder.transform.parent = transform;

                //index folders and prefab dict by name
                folders.Add(folder.name, folder);
            }
            prefabDict.Add(prefab.name, prefab);
        }
    }

    public TEntity Create<TEntity>() where TEntity : Entity
    {
        string typeName = typeof(TEntity).Name;
        string plural = ToPlural(typeName);

        GameObject gameObj = Instantiate(prefabDict[typeName], folders[plural].transform);

        //entity's should have sprite renderer child
        SpriteRenderer sr = gameObj.GetComponentInChildren<SpriteRenderer>();
        sr.sortingOrder = (folders[plural].transform.childCount - 1);

        return gameObj.GetComponent<TEntity>();
    }

    public GameObject Create(EntityType entityType)
    {
        string plural = ToPlural(entityType.DisplayName());
        GameObject gameObj = Instantiate(prefabDict[entityType.DisplayName()], folders[plural].transform);

        //entity's should have sprite renderer child
        SpriteRenderer sr = gameObj.GetComponentInChildren<SpriteRenderer>();
        sr.sortingOrder = (folders[plural].transform.childCount-1);

        return gameObj;
    }

    private string ToPlural(string name)
    {
        return (name[name.Length - 1] == 's') ? name + "es" : name + "s";
    }

    public TEntity[] GetAll<TEntity>() where TEntity : Entity
    {
        string typeName = typeof(TEntity).Name;
        GameObject folder = folders[ToPlural(typeName)];
        if (folder == null)
        {
            return null;
        }

        TEntity[] entities = new TEntity[folder.transform.childCount];
        for (int i = 0; i < folder.transform.childCount; i++)
        {
            entities[i] = folder.transform.GetChild(i).GetComponent<TEntity>();
        }

        return entities;
    }

    public Player GetPlayer()
    {
        return GetAll<Player>()[0];
    }
}
