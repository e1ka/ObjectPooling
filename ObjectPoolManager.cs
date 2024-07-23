using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPoolManager : MonoBehaviour
{
    public enum PoolType {Enemy, Bullet, None}
    private GameObject _objectPoolsEmptyHolder;
    private static GameObject _enemiesEmpty;
    private static GameObject _bulletsEmpty;
    public static List<PooledObjectInfo> ObjectPools = new List<PooledObjectInfo>();
    private void Awake() {
        _objectPoolsEmptyHolder = new GameObject("Pooled Objects");
        _enemiesEmpty = new GameObject("Enemies");
        _enemiesEmpty.transform.SetParent(_objectPoolsEmptyHolder.transform);
        _bulletsEmpty = new GameObject("Bullets");
        _bulletsEmpty.transform.SetParent(_objectPoolsEmptyHolder.transform);
    }
    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPosition, Quaternion spawnRotation, PoolType type = PoolType.None)
    {
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == objectToSpawn.name);
        if(pool == null)
        {
            pool = new PooledObjectInfo() {LookupString = objectToSpawn.name};
            ObjectPools.Add(pool);
        }

        GameObject spawnableObj = pool.InactiveObjects.FirstOrDefault();

        if(spawnableObj == null)
        {
            GameObject parentObj = SetParentObject(type);

            spawnableObj = Instantiate(objectToSpawn, spawnPosition, spawnRotation);
            
            if(parentObj != null)
            {
                spawnableObj.transform.SetParent(parentObj.transform);
            }
            
        }
        else
        {
            spawnableObj.transform.position = spawnPosition;
            spawnableObj.transform.rotation = spawnRotation;
            pool.InactiveObjects.Remove(spawnableObj);
            spawnableObj.SetActive(true);
        }

        return spawnableObj;
    }

    public static void ReturnObjectToPool(GameObject obj)
    {
        string goName = obj.name.Substring(0, obj.name.Length - 7); //removing the (Clone) from name
        PooledObjectInfo pool = ObjectPools.Find(p => p.LookupString == goName);
        obj.SetActive(false);
        if(pool == null)
        {
            Debug.Log("Relase object that is not pooled " + obj.name);
        }
        else
        {
            pool.InactiveObjects.Add(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.Enemy:
                return _enemiesEmpty;
            case PoolType.Bullet:
                return _bulletsEmpty;
            case PoolType.None:
                return null;
            default:
                return null;
        }
    }

    public class PooledObjectInfo
    {
        public string LookupString;
        public List<GameObject> InactiveObjects = new List<GameObject>();
    }
}