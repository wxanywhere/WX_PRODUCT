
#r "System.Xml.dll"
#r "System.Xml.Linq.dll"
#r "PresentationCore.dll"
#r "PresentationFramework.dll"
#r "WindowsBase.dll"
open System
open System.Xml
open System.Xml.XPath
open System.Xml.Linq
open System.Windows.Data

#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\ClientDebug"
#I  @"D:\Workspace\SBIIMS\SBIIMS_Assemblies\UtilityDebug"
#r "WX.Data.Helper.dll"
#r "WX.Data.dll"
open WX.Data.Helper
open WX.Data


//==========================================



//==========================================

match new XmlDocument() with
| y ->
    match y.CreateXmlDeclaration("1.0","UTF-8",null) with
    | z ->
        y.AppendChild z|>ignore
        y.OuterXml

let writer = XmlWriter.Create(@"d:\employees1.xml")
writer.WriteStartDocument()
writer.WriteStartElement("Employees")
writer.WriteStartElement("Employee")
//writer.WriteStartAttribute ("C_ID","wx")
writer.WriteAttributeString("C_ID","wx")
//writer.WriteElementString("ID", "wx")
writer.Close()


let w=XmlWriter.Create("")

    match new XmlDocument() with
    | x ->
        match x.CreateElement("Columns") with
        | y ->
            "C_ID"
            "C_XH"
            "C_MKID"
            "C_BGM"
            "C_KJBZ"
            "C_LLX"
            "C_LK"
            "C_BDALX"
            "C_ZBDBLX"
            "C_LBT"
            "C_LBTJM"
            "C_LTBDYSM"
            "C_LTBDA"
            "C_LTBDB"
            "C_BDA"
            "C_BDB"
            "C_BDC"
            "C_ZBDSJY"
            "C_ZBDA"
            "C_ZBDB"
            "C_BZ"


y.C_ID<- match a.Attributes.["C_ID"] with
y.C_XH<- match a.Attributes.["C_XH"] with 
y.C_MKID<- match a.Attributes.["C_MKID"] with 
y.C_BGM<- match a.Attributes.["C_BGM"] with 
y.C_KJBZ<- match a.Attributes.["C_KJBZ"] with 
y.C_LLX<- match a.Attributes.["C_LLX"] with 
y.C_LK<- match a.Attributes.["C_LK"] with 
y.C_BDALX<- match a.Attributes.["C_BDALX"] with 
y.C_ZBDBLX<- match a.Attributes.["C_ZBDBLX"] with 
y.C_LBT<- match a.Attributes.["C_LBT"] with 
y.C_LBTJM<- match a.Attributes.["C_LBTJM"] with 
y.C_LTBDYSM<- match a.Attributes.["C_LTBDYSM"] with 
y.C_LTBDA<- match a.Attributes.["C_LTBDA"] with 
y.C_LTBDB<- match a.Attributes.["C_LTBDB"] with 
y.C_BDA<- match a.Attributes.["C_BDA"] with 
y.C_BDB<- match a.Attributes.["C_BDB"] with 
y.C_BDC<- match a.Attributes.["C_BDC"] with 
y.C_ZBDSJY<- match a.Attributes.["C_ZBDSJY"] with 
y.C_ZBDA<- match a.Attributes.["C_ZBDA"] with 
y.C_ZBDB<- match a.Attributes.["C_ZBDB"] with 
y.C_BZ<- match a.Attributes.["C_BZ"] with 





a.Attributes.["C_ID"]
a.Attributes.["C_XH"]
a.Attributes.["C_MKID"]
a.Attributes.["C_BGM"]
a.Attributes.["C_KJBZ"]
a.Attributes.["C_LLX"]
a.Attributes.["C_LK"]
a.Attributes.["C_BDALX"]
a.Attributes.["C_ZBDBLX"]
a.Attributes.["C_LBT"]
a.Attributes.["C_LBTJM"]
a.Attributes.["C_LTBDYSM"]
a.Attributes.["C_LTBDA"]
a.Attributes.["C_LTBDB"]
a.Attributes.["C_BDA"]
a.Attributes.["C_BDB"]
a.Attributes.["C_BDC"]
a.Attributes.["C_ZBDSJY"]
a.Attributes.["C_ZBDA"]
a.Attributes.["C_ZBDB"]
a.Attributes.["C_BZ"]


y.C_ID<-a.C_ID
y.C_XH<-a.C_XH
y.C_MKID<-a.C_MKID
y.C_BGM<-a.C_BGM
y.C_KJBZ<-a.C_KJBZ
y.C_LLX<-a.C_LLX
y.C_LK<-a.C_LK
y.C_BDALX<-a.C_BDALX
y.C_ZBDBLX<-a.C_ZBDBLX
y.C_LBT<-a.C_LBT
y.C_LBTJM<-a.C_LBTJM
y.C_LTBDYSM<-a.C_LTBDYSM
y.C_LTBDA<-a.C_LTBDA
y.C_LTBDB<-a.C_LTBDB
y.C_BDA<-a.C_BDA
y.C_BDB<-a.C_BDB
y.C_BDC<-a.C_BDC
y.C_ZBDSJY<-a.C_ZBDSJY
y.C_ZBDA<-a.C_ZBDA
y.C_ZBDB<-a.C_ZBDB
y.C_BZ<-a.C_BZ









































"C_ID",string n.C_ID
"C_XH",string n.C_XH
"C_MKID",string n.C_MKID
"C_BGM",string n.C_BGM
"C_KJBZ",string n.C_KJBZ
"C_LLX",string n.C_LLX
"C_LK",string n.C_LK
"C_BDALX",string n.C_BDALX
"C_ZBDBLX",string n.C_ZBDBLX
"C_LBT",string n.C_LBT
"C_LBTJM",string n.C_LBTJM
"C_LTBDYSM",string n.C_LTBDYSM
"C_LTBDA",string n.C_LTBDA
"C_LTBDB",string n.C_LTBDB
"C_BDA",string n.C_BDA
"C_BDB",string n.C_BDB
"C_BDC",string n.C_BDC
"C_ZBDSJY",string n.C_ZBDSJY
"C_ZBDA",string n.C_ZBDA
"C_ZBDB",string n.C_ZBDB
"C_BZ",string n.C_BZ


w.WriteAttributeString ("C_ID",string n.C_ID)
w.WriteAttributeString ("C_XH",string n.C_XH)
w.WriteAttributeString ("C_MKID",string n.C_MKID)
w.WriteAttributeString ("C_BGM",string n.C_BGM)
w.WriteAttributeString ("C_KJBZ",string n.C_KJBZ)
w.WriteAttributeString ("C_LLX",string n.C_LLX)
w.WriteAttributeString ("C_LK",string n.C_LK)
w.WriteAttributeString ("C_BDALX",string n.C_BDALX)
w.WriteAttributeString ("C_ZBDBLX",string n.C_ZBDBLX)
w.WriteAttributeString ("C_LBT",string n.C_LBT)
w.WriteAttributeString ("C_LBTJM",string n.C_LBTJM)
w.WriteAttributeString ("C_LTBDYSM",string n.C_LTBDYSM)
w.WriteAttributeString ("C_LTBDA",string n.C_LTBDA)
w.WriteAttributeString ("C_LTBDB",string n.C_LTBDB)
w.WriteAttributeString ("C_BDA",string n.C_BDA)
w.WriteAttributeString ("C_BDB",string n.C_BDB)
w.WriteAttributeString ("C_BDC",string n.C_BDC)
w.WriteAttributeString ("C_ZBDSJY",string n.C_ZBDSJY)
w.WriteAttributeString ("C_ZBDA",string n.C_ZBDA)
w.WriteAttributeString ("C_ZBDB",string n.C_ZBDB)
w.WriteAttributeString ("C_BZ",string n.C_BZ)
, ""
, ""
, ""
, ""
, ""
, ""
, ""



  let ExportToXmlDoc ()=
    match new XmlDocument() with
    | x ->
        x.crea
        x.CreateXmlDeclaration("1.0", "utf-8", null)|>ignore
        x.Save (@"d:\Test1.xml")
        x.ins
        match x.CreateElement( "Column") with
        | y ->
            y.Attributes.Append(x.CreateAttribute "C_ID") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_XH") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_MKID") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_BGM") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_KJBZ") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LLX") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LK") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_BDALX") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_ZBDBLX") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LBT") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LBTJM") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LTBDYSM") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LTBDA") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_LTBDB") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_BDA") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_BDB") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_BDC") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_ZBDSJY") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_ZBDA") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_ZBDB") |>ignore
            y.Attributes.Append(x.CreateAttribute  "C_BZ") |>ignore
            x.AppendChild y|>ignore
            //root.AppendChild x|>ignore
        x.Save (@"d:\Test.xml")

File.Exists @"d:\Test.xml"

//========================================================
(*
C#中XML操作,创建.添加,修改,删除.2010-04-08 13:54using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Text;

public partial class _2008_Default2 : System.Web.UI.Page
{
    XmlDocument xmldoc;
    XmlNode xmlnode;
    XmlElement xmlelem;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    private void makeXML()
    {
        xmldoc = new XmlDocument();
        //加入XML的声明段落,<?xml version="1.0" encoding="gb2312"?>
        XmlDeclaration xmldecl;
        xmldecl = xmldoc.CreateXmlDeclaration("1.0", "gb2312", null);
        xmldoc.AppendChild(xmldecl);

        //加入一个根元素
        xmlelem = xmldoc.CreateElement("", "Employees", "");
        xmldoc.AppendChild(xmlelem);
        //加入另外一个元素
        for (int i = 0; i < 2; i++)
        {

            XmlNode root = xmldoc.SelectSingleNode("Employees");//查找<Employees> 
            XmlElement xe1 = xmldoc.CreateElement("Node");//创建一个<Node>节点 
            xe1.SetAttribute("genre", "李赞红");//设置该节点genre属性 
            xe1.SetAttribute("ISBN", "2-3631-4");//设置该节点ISBN属性

            XmlElement xesub1 = xmldoc.CreateElement("title");
            xesub1.InnerText = "CS从入门到精通";//设置文本节点 
            xe1.AppendChild(xesub1);//添加到<Node>节点中 
            XmlElement xesub2 = xmldoc.CreateElement("author");
            xesub2.InnerText = "候捷";
            xe1.AppendChild(xesub2);
            XmlElement xesub3 = xmldoc.CreateElement("price");
            xesub3.InnerText = "58.3";
            xe1.AppendChild(xesub3);

            root.AppendChild(xe1);//添加到<Employees>节点中 
        }
        //保存创建好的XML文档
        xmldoc.Save(Server.MapPath("data.xml")); 
    }

    private void makeXMl2()
    {
        XmlTextWriter xmlWriter;
        string strFilename = Server.MapPath("data1.xml");

        xmlWriter = new XmlTextWriter(strFilename, Encoding.Default);//创建一个xml文档
        xmlWriter.Formatting = Formatting.Indented;
        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("Employees");

        xmlWriter.WriteStartElement("Node");
        xmlWriter.WriteAttributeString("genre", "李赞红");
        xmlWriter.WriteAttributeString("ISBN", "2-3631-4");

        xmlWriter.WriteStartElement("title");
        xmlWriter.WriteString("CS从入门到精通");
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("author");
        xmlWriter.WriteString("候捷");
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("price");
        xmlWriter.WriteString("58.3");
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndElement();

        xmlWriter.Close();

    }

    protected void Button1_Click(object sender, EventArgs e)
    {
        makeXML();
        makeXMl2();
    }

    private void addxmldata()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Server.MapPath("data.xml"));
        XmlNode root = xmlDoc.SelectSingleNode("Employees");//查找<Employees> 
        XmlElement xe1 = xmlDoc.CreateElement("Node");//创建一个<Node>节点 
        xe1.SetAttribute("genre", "张三");//设置该节点genre属性 
        xe1.SetAttribute("ISBN", "1-1111-1");//设置该节点ISBN属性

        XmlElement xesub1 = xmlDoc.CreateElement("title");
        xesub1.InnerText = "C#入门帮助";//设置文本节点 
        xe1.AppendChild(xesub1);//添加到<Node>节点中 
        XmlElement xesub2 = xmlDoc.CreateElement("author");
        xesub2.InnerText = "高手";
        xe1.AppendChild(xesub2);
        XmlElement xesub3 = xmlDoc.CreateElement("price");
        xesub3.InnerText = "158.3";
        xe1.AppendChild(xesub3);

        root.AppendChild(xe1);//添加到<Employees>节点中 
        xmlDoc.Save(Server.MapPath("data.xml"));


    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        addxmldata();
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        deleteXML();
    }
    private void deleteXML()//删除结点
    {
        XmlDocument xmlDoc=new XmlDocument(); 
        xmlDoc.Load( Server.MapPath("data.xml") ); 
        XmlNode root=xmlDoc.SelectSingleNode("Employees");
        XmlNodeList xnl = xmlDoc.SelectSingleNode("Employees").ChildNodes;
        for (int i = 0; i < xnl.Count; i++)
        {
            XmlElement xe = (XmlElement)xnl.Item(i);
            if (xe.GetAttribute("genre") == "张三")
            {
                root.RemoveChild(xe);
                if (i < xnl.Count) i = i - 1;
            }
        }
        xmlDoc.Save(Server.MapPath("data.xml")); 
    }

    private void updateXML()//修改结点的值（属性和子结点）
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Server.MapPath("data.xml"));

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("Employees").ChildNodes;//获取Employees节点的所有子节点

        foreach (XmlNode xn in nodeList)//遍历所有子节点 
        {
            XmlElement xe = (XmlElement)xn;//将子节点类型转换为XmlElement类型 
            if (xe.GetAttribute("genre") == "张三")//如果genre属性值为“张三” 
            {
                xe.SetAttribute("genre", "update张三");//则修改该属性为“update张三”

                XmlNodeList nls = xe.ChildNodes;//继续获取xe子节点的所有子节点 
                foreach (XmlNode xn1 in nls)//遍历 
                {
                    XmlElement xe2 = (XmlElement)xn1;//转换类型 
                    if (xe2.Name == "author")//如果找到 
                    {
                        xe2.InnerText = "亚胜";//则修改
                    }
                }
            }
        }
        xmlDoc.Save(Server.MapPath("data.xml"));//保存。

    }

    protected void Button4_Click(object sender, EventArgs e)
    {
        updateXML();
    }

    protected void Button5_Click(object sender, EventArgs e)
    {
        updateXML2();
    }

    private void updateXML2()//修改结点（添加结点的属性和添加结点的自结点
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Server.MapPath("data.xml"));

        XmlNodeList nodeList = xmlDoc.SelectSingleNode("Employees").ChildNodes;//获取Employees节点的所有子节点

        foreach (XmlNode xn in nodeList)
        {
            XmlElement xe = (XmlElement)xn;
            xe.SetAttribute("test", "111111");

            XmlElement xesub = xmlDoc.CreateElement("flag");
            xesub.InnerText = "1";
            xe.AppendChild(xesub);
        }
        xmlDoc.Save(Server.MapPath("data.xml"));
    }


    protected void Button6_Click(object sender, EventArgs e)
    {
        RemoveChild();
    }

    private void RemoveChild()//删除结点中的某一个属性
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(Server.MapPath("data.xml"));
        XmlNodeList xnl = xmlDoc.SelectSingleNode("Employees").ChildNodes;
        foreach (XmlNode xn in xnl)
        {
            XmlElement xe = (XmlElement)xn;
            xe.RemoveAttribute("genre");//删除genre属性

            XmlNodeList nls = xe.ChildNodes;//继续获取xe子节点的所有子节点 
            foreach (XmlNode xn1 in nls)//遍历 
            {
                XmlElement xe2 = (XmlElement)xn1;//转换类型 
                if (xe2.Name == "flag")//如果找到 
                {
                    xe.RemoveChild(xe2);//则删除
                }
            }
        }
        xmlDoc.Save(Server.MapPath("data.xml"));
    }

    private void readXMl()//按照文本文件读取xml
    {
        System.IO.StreamReader myFile = new System.IO.StreamReader(Server.MapPath("data.xml"), System.Text.Encoding.Default);
        //注意System.Text.Encoding.Default

        string myString = myFile.ReadToEnd();//myString是读出的字符串
        myFile.Close();

        Response.Write(myString);

    }
    protected void Button7_Click(object sender, EventArgs e)
    {
        readXMl();
    }
}
 

*)