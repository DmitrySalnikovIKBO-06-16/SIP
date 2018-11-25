using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XMLLoad : MonoBehaviour {

    public struct Ammo
    {
        public int damage;
    }
    
    public Dictionary<string, Player.Weapon_Param> weaponList;
    public Dictionary<string, Ammo> ammoList;

    // Use this for initialization
    void Start ()
    {
        weaponList = new Dictionary<string, Player.Weapon_Param>();
        ammoList = new Dictionary<string, Ammo>();
        ReadXML();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void ReadXML()
    {
        XmlDocument xmlDoc = new XmlDocument();
        ReadXMLAmmo(xmlDoc);
    //public Dictionary<string, Weapon_Param> weaponList;
    TextAsset xmlAsset = Resources.Load<TextAsset>("Weapons"); //Считываем первоначальный файл

        if (xmlAsset)
            xmlDoc.LoadXml(xmlAsset.text); //пытаемся открыть документ
        else
        {
            Debug.LogError("NOT Found");
            return; //Если не открыли, то больше делать особо нечего
        }
        XmlNodeList inclList = xmlDoc.GetElementsByTagName("include"); //Смотрим все инклюды (инклюды писать ТОЛЬКО в первоначальный файл!)
        List<XmlDocument> docList = new List<XmlDocument>();//список документов, с которыми будем работать


        XmlDocument xmlDocIncl;
        foreach (XmlNode item in inclList)
        {
            xmlAsset = Resources.Load<TextAsset>(item.InnerText); //Считываем инклюды 
            xmlDocIncl = new XmlDocument();
            if (xmlAsset)
            {
                xmlDocIncl.LoadXml(xmlAsset.text); //Подгружаем удачные (лишние инклюды программу не крашнут и будут тупо игнорироваться)
               // Debug.Log("Loaded incl: " + item.InnerText);
            }
            else
            {
               // Debug.Log("NOT Found: " + item.InnerText);
            }

            docList.Add(xmlDocIncl); //Записываем в списочек
        }
        Player.Weapon_Param weaponParam;
        string weaponName;
        Ammo am;

        foreach (XmlDocument doc in docList) //По всем документам
        {
            XmlNodeList dataList = doc.GetElementsByTagName("item");

            foreach (XmlNode item in dataList) //По всем Item'ам
            {
                XmlNodeList itemContent = item.ChildNodes;
                weaponName = item.Attributes["name"].Value;

                if (weaponList.ContainsKey(weaponName))
                {
                    Debug.LogError(weaponName + " Error - already contains"); //Если у нас уже есть такое оружие, то недовольно пишем в консоль "Что за херня?!"
                }
                else
                {
                    weaponParam = new Player.Weapon_Param();
                    foreach (XmlNode itemItens in itemContent) //По всем внутренним параметрам Item'а
                    {
                        if (itemItens.Name == "Slot")
                            System.Int32.TryParse(itemItens.InnerText, out weaponParam.Slot); //Считываем параметры
                        if (itemItens.Name == "Ammo")
                        {
                            weaponParam.Ammo = itemItens.InnerText; //Считываем параметры
                            if (ammoList.TryGetValue(itemItens.InnerText, out am)) //берем из списка патронов всякие параметры (урон пока только)
                                weaponParam.Damage = am.damage; // записываем их
                            else
                            {
                                weaponParam.Damage = 1; //Урон, если с патронами определиться не удалось
                                Debug.LogError(itemItens.InnerText + " Error - no such ammo found"); //Пишем в консоль, если не нашли таких патронов
                            }
                        }
                        if (itemItens.Name == "ClipSize")
                            System.Int32.TryParse(itemItens.InnerText, out weaponParam.ClipSize); //Считываем параметры
                       // if (itemItens.Name == "Damage")
                       //     System.Int32.TryParse(itemItens.InnerText, out weaponParam.Damage);
                        if (itemItens.Name == "Dist")
                            System.Int32.TryParse(itemItens.InnerText, out weaponParam.Dist);
                        if (itemItens.Name == "RPM")
                            System.Int32.TryParse(itemItens.InnerText, out weaponParam.RPM);
                        if (itemItens.Name == "Burst")
                            System.Int32.TryParse(itemItens.InnerText, out weaponParam.Burst);
                        if (itemItens.Name == "ReloadTime")
                            weaponParam.ReloadTime = System.Convert.ToDouble(itemItens.InnerText); //Здесь тоже, только функция конвертирования другая
                        if (itemItens.Name == "ReloadSound")
                            weaponParam.clip_reload = Resources.Load<AudioClip>(itemItens.InnerText);
                        if (itemItens.Name == "ShotSound")
                            weaponParam.clip_shot = Resources.Load<AudioClip>(itemItens.InnerText);
                    }
                    weaponList.Add(weaponName, weaponParam); //Запихиваем все в оперативку, она резиновая, сдюжит
                }

            }
        }


    }

    void ReadXMLAmmo(XmlDocument xmlDoc) //По факту копия предыдущей функции, но для патронов
    {
        //public Dictionary<string, Weapon_Param> weaponList;
        TextAsset xmlAsset = Resources.Load<TextAsset>("Ammo"); //Считываем первоначальный файл

        if (xmlAsset)
            xmlDoc.LoadXml(xmlAsset.text); //пытаемся открыть документ
        else
        {
            Debug.LogError("NOT Found");
            return; //Если не открыли, то больше делать особо нечего
        }
        XmlNodeList inclList = xmlDoc.GetElementsByTagName("include"); //Смотрим все инклюды (инклюды писать ТОЛЬКО в первоначальный файл!)
        List<XmlDocument> docList = new List<XmlDocument>();//список документов, с которыми будем работать


        XmlDocument xmlDocIncl;
        foreach (XmlNode item in inclList)
        {
            xmlAsset = Resources.Load<TextAsset>(item.InnerText); //Считываем инклюды 
            xmlDocIncl = new XmlDocument();
            if (xmlAsset)
            {
                xmlDocIncl.LoadXml(xmlAsset.text); //Подгружаем удачные (лишние инклюды программу не крашнут и будут тупо игнорироваться)
                //Debug.Log("Loaded incl: " + item.InnerText);
            }
            else
            {
                //Debug.Log("NOT Found: " + item.InnerText);
            }

            docList.Add(xmlDocIncl); //Записываем в списочек
        }
        Ammo ammoParam;
        string ammoName;

        foreach (XmlDocument doc in docList) //По всем документам
        {
            XmlNodeList dataList = doc.GetElementsByTagName("item");

            foreach (XmlNode item in dataList) //По всем Item'ам
            {
                XmlNodeList itemContent = item.ChildNodes;
                ammoName = item.Attributes["name"].Value;

                if (weaponList.ContainsKey(ammoName))
                {
                    Debug.LogError(ammoName + " Error - already contains"); //Если у нас уже есть такие патроны, то недовольно пишем в консоль "Что за херня?!"
                }
                else
                {
                    ammoParam = new Ammo();
                    foreach (XmlNode itemItens in itemContent) //По всем внутренним параметрам Item'а
                    {
                        if (itemItens.Name == "Damage")
                            System.Int32.TryParse(itemItens.InnerText, out ammoParam.damage);
                    }
                    ammoList.Add(ammoName, ammoParam); //Запихиваем все в оперативку, она резиновая, сдюжит
                }

            }
        }


    }
}
