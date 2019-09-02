using ETModel;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ETHotfix
{

    public struct DBArea
    {
        public int isNl;//1=直辖市   北京 上海
        public string name_zh;
        public string name_en;
        public int id;
        public bool walletLoad;//是否为钱包load出来的.默认为false
    }

    public class DBAreaSet
    {
        public List<DBArea> data;
    }

    [ObjectSystem]
    public class DBAreaComponentAwakeSystem : AwakeSystem<DBAreaComponent>
    {
        public override void Awake(DBAreaComponent self)
        {
            self.Awake();
        }
    }

    public class DBAreaComponent : Component
    {

        public static DBAreaComponent Instance;
        DBAreaSet areasSet;
        DBAreaSet provincesSet;
        DBAreaSet citysSet;
        DBAreaSet countrysSet;
        public void Awake()
        {
            Instance = this;
            ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
            resourcesComponent.LoadBundle("areaconfig.unity3d");
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("areaconfig.unity3d", "AreaConfig");

            TextAsset txt = bundleGameObject.GetComponent<ReferenceCollector>().Get<TextAsset>("areas");
            areasSet = JsonHelper.FromJson<DBAreaSet>(txt.text);
            txt = bundleGameObject.GetComponent<ReferenceCollector>().Get<TextAsset>("provinces");
            provincesSet = JsonHelper.FromJson<DBAreaSet>(txt.text);
            txt = bundleGameObject.GetComponent<ReferenceCollector>().Get<TextAsset>("citys");
            citysSet = JsonHelper.FromJson<DBAreaSet>(txt.text);
            txt = bundleGameObject.GetComponent<ReferenceCollector>().Get<TextAsset>("countrys");
            countrysSet = JsonHelper.FromJson<DBAreaSet>(txt.text);
        }

        /// <summary>
        /// 获取地区列表
        /// </summary>
        /// <returns></returns>
        public List<DBArea> GetAreaList()
        {
            return areasSet.data;
        }

        /// <summary>
        /// 获取地区内的国家列表
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public List<DBArea> GetCountryList(int _id)
        {
            List<DBArea> countrys = new List<DBArea>();
            string strid;
            for (int i = 0; i < countrysSet.data.Count; ++i)
            {
                strid = countrysSet.data[i].id.ToString();
                if (strid.IndexOf(_id.ToString()) == 0)
                {
                    countrys.Add(countrysSet.data[i]);
                }
            }
            return countrys;
        }

        /// <summary>
        /// 获取省列表  默认中国
        /// </summary>
        /// <param name="_lan"></param>
        /// <returns></returns>
        public List<DBArea> GetProvinceList()
        {
            return provincesSet.data;
        }

        /// <summary>
        /// 根据省份ID获取对应的城市列表
        /// </summary>
        /// <param name="_provincesId"></param>
        /// <returns></returns>
        public List<DBArea> GetCityList(int _provincesId)
        {
            List<DBArea> citys = new List<DBArea>();
            string strid;
            for (int i = 0; i < citysSet.data.Count; ++i)
            {
                strid = citysSet.data[i].id.ToString();
                if (strid.IndexOf(_provincesId.ToString()) == 0)
                {
                    citys.Add(citysSet.data[i]);
                }
            }
            return citys;
        }


        public string QueryCity(string _strId)
        {
            int id;
            if (int.TryParse(_strId, out id))
            {

                return QueryCity(id);
            }
            return "unknow";
        }


        /// <summary>
        /// 根据ID搜索城市名字
        /// </summary>
        /// <param name="_id">地区ID</param>
        /// <param name="_lan">语言种类</param>
        /// <returns></returns>
        public string QueryCity(int _id)
        {
            for (int i = 0; i < citysSet.data.Count; ++i)
            {
                if (citysSet.data[i].id == _id)
                {

                    return LanguageMgr.mInstance.GetLanguageForKey($"area{_id}");
                }
            }

            for (int i = 0; i < provincesSet.data.Count; ++i)
            {
                if (provincesSet.data[i].id == _id)
                {
                    return LanguageMgr.mInstance.GetLanguageForKey($"area{_id}");
                }
            }

            for (int i = 0; i < countrysSet.data.Count; ++i)
            {
                if (countrysSet.data[i].id == _id)
                {
                    return LanguageMgr.mInstance.GetLanguageForKey($"area{_id}");
                }
            }
            return "unknow";
        }

        /// <summary>
        /// 得到城市 struct
        /// </summary>
        public DBArea QueryCityByWallet(string id)
        {
            for (int i = 0; i < citysSet.data.Count; ++i)
            {
                if (citysSet.data[i].id == int.Parse(id))
                {
                    return citysSet.data[i];
                }
            }
            return new DBArea();
        }

        /// <summary>
        /// 得到省份 struct
        /// </summary>
        public DBArea QueryProvincesByWallet(string id)
        {
            for (int i = 0; i < provincesSet.data.Count; ++i)
            {
                if (provincesSet.data[i].id == int.Parse(id))
                {
                    return provincesSet.data[i];
                }
            }
            return new DBArea();
        }



        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
