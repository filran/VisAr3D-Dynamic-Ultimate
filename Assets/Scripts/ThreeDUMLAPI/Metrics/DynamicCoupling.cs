using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ThreeDUMLAPI;
using UnityEngine;
using System.Globalization;

public static class DynamicCoupling
{
    //FAKE
    private static Dictionary<IXmlNode,GameObject> Lifelines()
    {
        //LIFE ONE
        Method m11 = new Method();
        m11.Id = "method11";
        m11.Label = "method11()";
        m11.IdSource = "idum";
        m11.IdTarget = "iddois";

        Method m12 = new Method();
        m12.Id = "method12";
        m12.Label = "method12()";
        m12.IdSource = "idum";
        m12.IdTarget = "iddois";

        Method m13 = new Method();
        m13.Id = "method13";
        m13.Label = "method13()";
        m13.IdSource = "idum";
        m13.IdTarget = "iddois";

        SoftwareEntity l1 = new Lifeline();
        l1.Id = "idum";
        l1.Name = "UM";
        l1.Methods.Add(m11);
        l1.Methods.Add(m12);
        l1.Methods.Add(m13);

        GameObject go_life1 = new GameObject();

        //LIFE TWO
        Method m21 = new Method();
        m21.Id = "method21";
        m21.Label = "method21()";
        m21.IdSource = "iddois";
        m21.IdTarget = "idum";

        Method m22 = new Method();
        m22.Id = "method22";
        m22.Label = "method22()";
        m22.IdSource = "iddois";
        m22.IdTarget = "idum";

        SoftwareEntity l2 = new Lifeline();
        l2.Id = "iddois";
        l2.Name = "DOIS";
        l2.Methods.Add(m21);
        l2.Methods.Add(m22);
        
        GameObject go_life2 = new GameObject();

        //LIFE THREE
        Method m31 = new Method();
        m31.Id = "method31";
        m31.Label = "method31()";
        m31.IdSource = "idtres";
        m31.IdTarget = "iddois";

        Method m32 = new Method();
        m32.Id = "method32";
        m32.Label = "method32()";
        m32.IdSource = "idtres";
        m32.IdTarget = "iddois";

        SoftwareEntity l3 = new Lifeline();
        l3.Id = "idtres";
        l3.Name = "TRES";
        l3.Methods.Add(m31);
        l3.Methods.Add(m32);

        GameObject go_life3 = new GameObject();
        
        //Lifelines
        Dictionary<IXmlNode, GameObject> Lifelines = new Dictionary<IXmlNode, GameObject>();
        Lifelines.Add(l1, go_life1);
        Lifelines.Add(l2, go_life2);
        Lifelines.Add(l3, go_life3);

        return Lifelines;
    }

    public static List<Dictionary<string, float>> eoc_ioc(Dictionary<Lifeline, GameObject> lifes , Dictionary<Method, GameObject> methods)
    {
        //Example
        //Dictionary<string, float> dic = new Dictionary<string, float>();
        //dic.Add(id_i , metric_i);
        //dic.Add(id_j , metric_j);
        //dic.Add("total" , total);
        //_dic_return_.Add(dic); ...
        //for each list, one dictionary with 3 index and his values to metric_i, metric_j and total
        List<Dictionary<string, float>> _dic_return_ = new List<Dictionary<string, float>>();
        Dictionary<string, float> factors = new Dictionary<string, float>(); //[idlifeline,totalmetric]

        string _return_ = "EOC_IOC\n";
        //Dictionary<IXmlNode, GameObject> lifes = Lifelines();
        
        //ALGORITM
        List<Lifeline> listLifelines = new List<Lifeline>();

        //1 create list of the Lifelines       
        var enumlifelines = lifes.GetEnumerator();
        while(enumlifelines.MoveNext())
        {
            listLifelines.Add((Lifeline)enumlifelines.Current.Key);
        }
                
        //2 select two lifelines: two for
        for (int i = 0; i < listLifelines.Count; i++) //lifeline 1
        {
            var enum_methods_i = listLifelines[i].Methods.GetEnumerator();

            for (int j = i + 1; j < listLifelines.Count; j++) //lifeline 2
            {
                var enum_methods_j = listLifelines[j].Methods.GetEnumerator();

                //3 verify if there is at least one msg betweens them
                //4 if yes, calculate the total of the msgs 
                float msgs_i = 0;
                float msgs_j = 0;
                float total = 0;
                float metric_i = 0;
                float metric_j = 0;

                while (enum_methods_i.MoveNext())
                {
                    if(enum_methods_i.Current.IdTarget == listLifelines[j].Id)
                    {
                        //_return_ += enum_methods_i.Current.Label + " aponta para " + listLifelines[j].Name + "\n";
                        msgs_i++;
                    }
                }

                while (enum_methods_j.MoveNext())
                {
                    if (enum_methods_j.Current.IdTarget == listLifelines[i].Id)
                    {
                        //_return_ += enum_methods_j.Current.Label + " aponta para " + listLifelines[i].Name + "\n";
                        msgs_j++;
                    }
                }
                
                total = msgs_i + msgs_j;
                metric_i = msgs_i / total;
                metric_j = msgs_j / total;

                Dictionary<string, float> dic = new Dictionary<string, float>();
                dic.Add(listLifelines[i].Id, metric_i);
                dic.Add(listLifelines[j].Id, metric_j);
                dic.Add("total", total);
                _dic_return_.Add(dic);

                if (total > 0)
                {
                    _return_ += listLifelines[i].Name + " e " + listLifelines[j].Name + "\n";
                    _return_ += "\t" + listLifelines[i].Name + " - msgs: " + msgs_i + " - Metric: " + metric_i + "\n";
                    _return_ += "\t" + listLifelines[j].Name + " - msgs: " + msgs_j + " - Metric: " + metric_j + "\n";
                    _return_ += "\tTotal: " + total + "\n";

                    foreach (KeyValuePair<Lifeline, GameObject> l in lifes)
                    {
                        if (l.Key.Id == listLifelines[i].Id)
                        {
                            Transform t = l.Value.transform.FindChild("object");
                            float x = t.localScale.x;
                            float y = t.localScale.y;
                            float z = t.localScale.z + metric_i / 3;
                            t.localScale = new Vector3(x,y,z);

                            Transform line = l.Value.transform.FindChild("line");
                            float line_x = line.localScale.x;
                            float line_y = line.localScale.y;
                            float line_z = line.localScale.z + metric_i / 3;
                            line.localScale = new Vector3(line_x, line_y, line_z);
                        }

                        if (l.Key.Id == listLifelines[j].Id)
                        {
                            Transform t = l.Value.transform.FindChild("object");
                            float x = t.localScale.x;
                            float y = t.localScale.y;
                            float z = t.localScale.z + metric_j;
                            t.localScale = new Vector3(x, y, z);

                            Transform line = l.Value.transform.FindChild("line");
                            float line_x = line.localScale.x;
                            float line_y = line.localScale.y;
                            float line_z = line.localScale.z + metric_j / 4;
                            line.localScale = new Vector3(line_x, line_y, line_z);
                        }
                    }

                    Factors(listLifelines[i].Id, metric_i, factors);
                    Factors(listLifelines[j].Id, metric_j, factors);

                    //foreach(KeyValuePair<Method,GameObject> m in methods)
                    //{
                    //    if(m.Key.IdSource == listLifelines[i].Id)
                    //    {
                    //Material material = m.Value.transform.FindChild("line").GetComponent<Renderer>().material;
                    //Debug.Log(m.Key.Label + " - " + (material.color.g*100));
                    //material.color = new Color(1, .848f-metric_i, 0, 1);

                    //float div = material.color.g / total;
                    //material.color = new Color(1, metric_i*div, 0,1);
                    //byte g = Convert.ToByte(255/Convert.ToInt32(metric_i));
                    //float factor = (1 * metric_i) / total;
                    //int g = ((255* Convert.ToInt32(factor)) / 100);
                    ////material.color = new Color32(255, Convert.ToByte(g-255), 0,255);
                    //Debug.Log(Convert.ToInt32(g));
                    //material.color = new Color(1,0.868f,0,1);

                    //}

                    //if (m.Key.IdSource == listLifelines[j].Id)
                    //{
                    //Material material = m.Value.transform.FindChild("line").GetComponent<Renderer>().material;
                    //Debug.Log(m.Key.Label + " - " + material.color);
                    //material.color = new Color(1, .848f - metric_j, 0, 1);

                    //float factor = (100 * metric_j) / total;

                    //Material material = m.Value.transform.FindChild("line").GetComponent<Renderer>().material;
                    ////Debug.Log(m.Key.Label+" - "+ material.color);
                    ////material.color = new Color(1, 0, 0, 1);

                    ////byte g = Convert.ToByte(255 / Convert.ToInt32(metric_j));
                    //material.color = new Color32(255, Convert.ToByte(total), 0, 255);
                    //    }
                    //}
                }

                //5 then, calculate metric for each lifeline
                //5.1 if one method there is target lifeline 2
                //if (enum_methods_i.Current.IdTarget == listLifelines[j].Id)
                //{
                //    _return_ += enum_methods_i.Current.Label + " aponta para " + listLifelines[j].Name + "\n";
                //    msgs_i++;
                //}

                ////5.2 if one method there is target lifeline 1
                //if (enum_methods_j.Current.IdTarget == listLifelines[i].Id)
                //{
                //    _return_ += enum_methods_j.Current.Label + " aponta para " + listLifelines[i].Name + "\n";
                //    msgs_j++;
                //}
            }
            _return_ += "\n\n";
        }
        //Example
        //12 13 14 15
        //23 24 25
        //34 35
        //45
        //for (int i = 1; i <= 5; i++ )
        //{
        //    for (int j = i+1; j <= 5; j++ )
        //    {
        //        _return_ += ""+i+j+" ";
        //    }
        //    _return_ += "\n";
        //}
        ListMetrics(lifes, factors);
        Debug.Log(_return_);
        return _dic_return_;
    }

    private static void Factors(string idlifeline , float m ,Dictionary<string,float> listlifelines)
    {
        float metric;
        var list = listlifelines.GetEnumerator();
        while(list.MoveNext())
        {
            //if there is lifeline, update metric
            if(idlifeline.Equals(list.Current.Key))
            {
                metric = list.Current.Value;
                listlifelines.Remove(list.Current.Key);
                listlifelines.Add(list.Current.Key, metric + m);
            }
            else
            {
                listlifelines.Add(list.Current.Key,m);
            }
        }
    }

    private static void ListMetrics(Dictionary<Lifeline,GameObject> lifelines,  Dictionary<string, float> metriclifelines)
    {
        string _return_ = "ListMetrics \n\n";
        var lifes = lifelines.GetEnumerator();
        while(lifes.MoveNext())
        {
            var metrics = metriclifelines.GetEnumerator();
            while(metrics.MoveNext())
            {
                if(lifes.Current.Key.Id.Equals(metrics.Current.Key))
                {
                    _return_ += lifes.Current.Key.Name + " : " + metrics.Current.Value + "\n";
                }
            }
        }
        Debug.Log(_return_);
    }
}