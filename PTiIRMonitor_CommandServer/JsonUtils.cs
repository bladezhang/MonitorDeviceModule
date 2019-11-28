﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using System.Security.Cryptography;
using Peiport_commandManegerSystem;
using System.Web.Script.Serialization;
namespace Peiport_commandManegerSystem
{
    public class JsonUtils
    {
        public Form1 frmMain;
        public struct stuReceiDeviceCodeCmdNum
        {
            public string code;
            public string codeName;
            public bool reDeviceImfFlag;
            public float reRelDataVal;
            public int reReAlarmVal;
            public DateTime dtReDeviceImfCmdTime;
            public DateTime dtReRelDataTime;
            public DateTime dtReAlarmTime;
        }
        public struct stuReceiDeviceCmdNum  //定义接收数据及命令缓存
        {
            public string devCode;
            public string devName;
            public List<stuReceiDeviceCodeCmdNum> lstPara;
        }
        public class Constant//消息类型常量类
        {
            public static int OTHER = 0;//其他类型指令
            public static int LOGIN = 1;//登录
            public static int PTZ = 2;//云台
            public static int IR = 3;//红外
            public static int VR = 4;//可见光
            public static int CRUISE = 5;//巡检
            public static int PTSP = 6;//rtsp视频播放
            public static string OK = "ok";
            public static string ERROR = "error";
        }
        private class MsgHeader
        {
            public string seq { get; set; }//标识

            public int cmdType { get; set; }//cmd命令类型,默认为-1,各个数字分别代表对应的cmd指令类型;

            public string cmdAction { get; set; }//具体指令信息

            public string result { get; set; }// 结果，只有当type为-1时，才有结果OK和error

            public string sender { get; set; }//发送指令的一方名称
            public string receiver { get; set; }//接收指令的一方名称
            public List<MesResponse> Params { get; set; }
        }
        private class MesResponse
        {
            public string Name { get; set; }
            public string sender { get; set; }
        }
        private class Cmd_Palpitate
        {
            public string seq { get; set; }//标识

            public int cmdType { get; set; }//cmd命令类型,默认为-1,各个数字分别代表对应的cmd指令类型;

            public string cmdAction { get; set; }//具体指令信息

            public string result { get; set; }// 结果，只有当type为-1时，才有结果OK和error

            public string sender { get; set; }//发送指令的一方名称
            public string receiver { get; set; }//接收指令的一方名称
            public List<Palpitate> Params { get; set; }
        }
        private class Palpitate
        {
            public string sender { get; set; }
        }
        //#region Json相关操作
        public List<stuReceiDeviceCmdNum> mlststuRecieDeviceCmdNum = new List<stuReceiDeviceCmdNum>();
        public void clsReceiDeviceCmdNumBuf()  //定接收缓冲
        {
            mlststuRecieDeviceCmdNum = new List<stuReceiDeviceCmdNum>();
        }
        string strDatetime = DateTime.Now.ToString("yyyyMMddhhssfff");
        public void CmdSend_Logout(string strName)//用户异地登录返回
        {
            try
            {
                JObject jobCmd = new JObject();
                jobCmd.Add(new JProperty("seq", strDatetime));
                jobCmd.Add(new JProperty("cmdType", 1));
                jobCmd.Add(new JProperty("cmdAction", "Logout"));
                jobCmd.Add(new JProperty("result", ""));
                jobCmd.Add(new JProperty("sender", "CmdServer"));
                jobCmd.Add(new JProperty("receiver", strName));
                JArray jarr = new JArray();
                JObject jobbuf = new JObject();
                jobbuf.Add(new JProperty("result", "该用户异地登录！！！"));
                jarr.Add(jobbuf);
                jobCmd.Add(new JProperty("paramList", jarr));
                string strjson = JsonConvert.SerializeObject(jobCmd);
                funSendOneFramCmd(strName, strjson);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Cmd_IRCamPaletteOpt_OK(string str, string strName)//登录成功命令返回
        {
            try
            {
                MsgHeader json = new MsgHeader();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Login";
                json.result = "OK";
                json.Params = new List<MesResponse>();
                json.Params.Add(new MesResponse { Name = strName, sender = str });
                json.sender = "CmdServer";
                json.receiver = strName;
                string strjson = JsonConvert.SerializeObject(json);
                funSendOneFramCmd(strName, strjson);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Cmd_IRCamPaletteOpt(string str, string strDizhi)//登录成功命令返回
        {
            try
            {
                MsgHeader json = new MsgHeader();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Login";
                json.result = "OK";
                json.Params = new List<MesResponse>();
                json.Params.Add(new MesResponse { sender = str });
                json.sender = "CmdServer";
                json.receiver = "user";
                string strjson = JsonConvert.SerializeObject(json);
                string strName = "admin";
                funSendOneFramCmd(strName, strjson);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Cmd_IRCamPaletteOpt_OUT(string str, string strDizhi, string strName)//登录失败命令返回
        {
            try
            {
                MsgHeader json = new MsgHeader();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Login";
                json.result = "OUT";
                json.Params = new List<MesResponse>();
                json.Params.Add(new MesResponse { Name = strName, sender = str });
                json.sender = "CmdServer";
                json.receiver = "user";
                string strjson = JsonConvert.SerializeObject(json);
                funSendOneFramCmd(strName, strjson);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public void Cmd_IRCamPaletteOpt_OK_n(string str)//list登录成功命令返回
        {
            try
            {
                MsgHeader json = new MsgHeader();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Login";
                json.result = "OK";
                json.Params = new List<MesResponse>();
                json.Params.Add(new MesResponse { sender = str });
                json.sender = "CmdServer";
                json.receiver = "user";
                string strjson = JsonConvert.SerializeObject(json);
                funSendOneFramCmd(str, strjson);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Cmd_IRCamPaletteOpt_reappear(string str1, string str)//用户名异地登录退出命令
        {
            try
            {
                MsgHeader json = new MsgHeader();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Logout";
                json.result = "OUT";
                json.Params = new List<MesResponse>();
                json.Params.Add(new MesResponse { sender = str1 });
                json.sender = "CmdServer";
                json.receiver = str;
                string strjson = JsonConvert.SerializeObject(json);
                funSendOneFramCmd_repeat(str, strjson);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Cmd_IRCamPaletteOpt_OUT(string str)//退出用户登录返回
        {
            try
            {
                MsgHeader json = new MsgHeader();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Logout";
                json.result = "OUT";
                json.Params = new List<MesResponse>();
                json.Params.Add(new MesResponse { sender = "退出成功" });
                json.sender = "CmdServer";
                json.receiver = str;
                string strjson = JsonConvert.SerializeObject(json);
                funSendOneFramCmd(str, strjson);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public void Cmd_IRCamPalpitate_OK(string str, string strname) //心跳命令返回
        {
            try
            {
                Cmd_Palpitate json = new Cmd_Palpitate();
                json.seq = DateTime.Now.ToString("yyyyMMddhhmmssfff");
                json.cmdType = 1;
                json.cmdAction = "Palpitate";
                json.result = "0";
                json.Params = new List<Palpitate>();
                json.Params.Add(new Palpitate { sender = str });
                json.sender = "CmdServer";
                json.receiver = strname;
                string strjson = JsonConvert.SerializeObject(json);
                funSendOneFramCmd(strname, strjson);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public struct stuWholeCmdBuf
        {
            public byte bt_RandCode;
            public byte[] bt4_SID;
            public int intContentLen;
            public byte[] bt_ReByteArray;
            public JObject jobjOneContent;
        }
        public struct stuReOneCmdBuf
        {
            public int iClientType;  //0== MonitorClient 1=userClent;
            public int iClientIndex;
            public string strClietIP;
            public int intClientPort;
            public stuWholeCmdBuf stuCmdBuf;
        }
        public struct stuSysVarImf  //与后台通讯用的总构体变量
        {
            public byte[] bt4_SID;      //通讯过程JSON用的变量
            public byte[] bt2_SOI;      //通讯过程JSON用的变量
            public byte btRandCode;     //通讯过程JSON用的变量

            public int intConnectStatus;  // 0-断开，1-连接，2-重连状态完成
            public int intLoginStatus;  //0-未登陆，1-登陆成功

        }
        public void clsClJsonCrcVarInit()
        {
            m_stuSystemVar.bt4_SID = new byte[4];
            m_stuSystemVar.bt4_SID[0] = 0x0;
            m_stuSystemVar.bt4_SID[1] = 0x0;
            m_stuSystemVar.bt4_SID[2] = 0x0;
            m_stuSystemVar.bt4_SID[3] = 0x0;
            m_stuSystemVar.bt2_SOI = new byte[2];
            m_stuSystemVar.bt2_SOI[0] = 0xA5;
            m_stuSystemVar.bt2_SOI[1] = 0x5A;

        }
        public struct stuReceiReturnCmdFormat
        {
            public string strResult;
            public string strMsg;
            public string strUser;
            public string strSeq;
            public string strSid;

        }
        public List<stuReceiReturnCmdFormat> m_lststuReturn = new List<stuReceiReturnCmdFormat>();
        public List<JObject> m_lst_JobjReturnCmdBuf = new List<JObject>();  //定义回归使用的Json结构体
        public struct stuBaseNumFrame//基础数据类定义
        {
            public byte[] bt2_SOI;
            public byte bt_RandCode;
            public byte[] bt4_SID;
            public byte[] bt4_ContentSize;  //发送命令时才计算
            public byte[] bt_Content;
            public byte bt_EOI;             //发送命令时才计算
        }
        List<byte> lstbt_ReByteBuf = new List<byte>();  //接收命令字节缓冲，内部使用
        public List<stuWholeCmdBuf> lststu_ReceiCmdBuf = new List<stuWholeCmdBuf>();  //接收到的命令缓冲
        public List<string> lststu_ReceiStrCmd = new List<string>();//存储转发命令
        stuWholeCmdBuf stu_ReceiProcessVar = new stuWholeCmdBuf(); //接收到中间变量 内部用
        public stuSysVarImf m_stuSystemVar = new stuSysVarImf();
        public clsCmdServerOpt clsCmdServer = new clsCmdServerOpt();
        public List<string> Str_Monitor = new List<string>();//接收
        public List<string> Li_MoitClient = new List<string>();//存储转发命令
        //  public List<string> StrNameUser = new List<string>();//存储用户姓名
        public List<string> StrUserdizhi = new List<string>();//存储用户地址
        public int lstintUserOut = 0;//0表示正在判断，1表示存在，2表示无用户，判断用户是否存在
        public bool funByteContentAnaToJsonObject(byte[] btBuf, ref JObject jobjRe)   //分解命令帧
        {
            bool blReFlag = false;
            jobjRe = new JObject();//清空
            string strBuf = System.Text.Encoding.UTF8.GetString(btBuf);
            string strBufN = JsonConvert.DeserializeObject(strBuf).ToString();
            try
            {
                jobjRe = JObject.Parse(strBufN);
                blReFlag = true;
            }
            catch (Exception)
            {
            }
            return blReFlag;
        }
        public bool funRecieveOneByteToBuf(byte btReBuf)   //收到一个字节内容
        {
            bool blRe = false;
            byte btbuf;
            lstbt_ReByteBuf.Add(btReBuf);
            int intReByteCount = lstbt_ReByteBuf.Count;
            if (intReByteCount <= 0)
            {
            }
            else if (intReByteCount == 1)
            {
                btbuf = 0xA5;// ;接收字头
                if (lstbt_ReByteBuf[0] != btbuf)
                {
                    lstbt_ReByteBuf.Clear();
                }
            }
            else if (intReByteCount == 2)
            {
                btbuf = 0x5A;//
                if (lstbt_ReByteBuf[1] != btbuf)
                {
                    lstbt_ReByteBuf.Clear();
                }
            }
            else if (intReByteCount == 3)  //接收随机码
            {
                stu_ReceiProcessVar = new stuWholeCmdBuf();
                stu_ReceiProcessVar.bt_RandCode = lstbt_ReByteBuf[2];
                stu_ReceiProcessVar.bt4_SID = new byte[4];
            }
            else if (intReByteCount == 11) //接收长度
            {
                long lng1;
                lng1 = (int)lstbt_ReByteBuf[10];
                lng1 = lng1 * 256 + (int)lstbt_ReByteBuf[9];
                lng1 = lng1 * 256 + (int)lstbt_ReByteBuf[8];
                lng1 = lng1 * 256 + (int)lstbt_ReByteBuf[7];
                if (lng1 > 65536)
                {

                    lstbt_ReByteBuf.Clear();
                }
                else
                {
                    stu_ReceiProcessVar.intContentLen = (int)lng1;
                }
            }
            else if (intReByteCount == (stu_ReceiProcessVar.intContentLen + 12))//收到最后一个数据
            {
                //校验一帧
                int intCrc, intCrc1;
                intCrc = 0x00;
                for (int i = 2; i < intReByteCount - 1; i++)
                {
                    intCrc = intCrc ^ lstbt_ReByteBuf[i];
                }
                intCrc = intCrc % 256;
                intCrc1 = (int)lstbt_ReByteBuf[intReByteCount - 1];
                if (intCrc == intCrc1)  //异域校验成功
                {
                    // 进入Json命令分解
                    int intCmdContentLen;
                    intCmdContentLen = stu_ReceiProcessVar.intContentLen;
                    if ((intCmdContentLen > 0) && (intCmdContentLen < 65536))
                    {
                        m_stuSystemVar.bt4_SID[0] = stu_ReceiProcessVar.bt4_SID[0];
                        m_stuSystemVar.bt4_SID[1] = stu_ReceiProcessVar.bt4_SID[1];
                        m_stuSystemVar.bt4_SID[2] = stu_ReceiProcessVar.bt4_SID[2];
                        m_stuSystemVar.bt4_SID[3] = stu_ReceiProcessVar.bt4_SID[3];

                        byte[] btbuf1 = new byte[intCmdContentLen];
                        // stuJasonData_One  mstuJsonOne=new stuJasonData_One ();
                        for (int i = 0; i < intCmdContentLen; i++)
                        {
                            btbuf1[i] = (byte)(lstbt_ReByteBuf[i + 11] ^ stu_ReceiProcessVar.bt_RandCode);
                        }
                        JObject jobre = new JObject();
                        stu_ReceiProcessVar.bt_ReByteArray = btbuf1;
                        if (funByteContentAnaToJsonObject(btbuf1, ref jobre) == true)
                        {
                            stu_ReceiProcessVar.jobjOneContent = jobre;
                        }
                        lststu_ReceiCmdBuf.Add(stu_ReceiProcessVar);
                        blRe = true;
                    }
                }

                //重新清一帧
                lstbt_ReByteBuf.Clear();
            }
            else if (intReByteCount > 50000)  //超长
            {
                //重新清一帧
                lstbt_ReByteBuf.Clear();
            }
            return blRe;
        }



        public void funreciCmdMonitor(string strDizhi) //转发
        {
            string strjson;
            for (int i = 0; i < frmMain.m_MOJsonCtrl.lststu_ReceiStrCmd.Count; i++)
            {
                strjson = frmMain.m_MOJsonCtrl.lststu_ReceiStrCmd[i];
                funSendOneFramCmd(strDizhi, strjson);
                return;
            }
        }
        public void CheckStaus(string strUser)//收到了命令立马回复，检验是否收到命令
        {
            JObject jobCmd = new JObject();
            jobCmd.Add(new JProperty("Server", "A5x0"));
            jobCmd.Add(new JProperty("sender", "CmdServer"));
            jobCmd.Add(new JProperty("receiver", strUser));
            JArray jarr = new JArray();
            JObject jobbuf = new JObject();
            jobbuf.Add(new JProperty("", ""));
            jarr.Add(jobbuf);
            jobCmd.Add(new JProperty("", jarr));
            string strjson = JsonConvert.SerializeObject(jobCmd);
            funSendOneFramCmd(strUser, strjson);
        }
        public void LoadUserUpdateByConnectImf(string strIP, int intPort, string strUserName)   //输入定时用户心跳更新
        {
            if (strUserName == "")
                return;

            DateTime dt = DateTime.Now;

            //检查是否存在相同的用户名在登陆，如果有，产生强制注销回调函数

            foreach (ListenClient ltc in frmMain.g_myServer.g_lstConnectClientTab)
            {
                if ((ltc.ipaEndIP.ToString() == strIP) && (ltc.intEndPort == intPort))//相同，即注册入去
                {
                    ltc.strLoginUserName = strUserName;
                    ltc.dtUpateLoadTime = dt;
                }
                else
                {
                    if (ltc.strLoginUserName == strUserName)//注销其它重名的连接
                    {
                        Cmd_IRCamPaletteOpt_reappear("用户异地登录！！！", strUserName);//返回admin用户在异地登录！！！Logout

                        ltc.CloseSocket();
                    }
                }
                return;
            }
        }
        public void funReceiCmdAnalyseScan() //定时100ms接收到命令解析 用于在中断线程内调用,解析完后，分别生对应的接收命令表
        {
            try
            {
                while (true)
                {

                    if (lststu_ReceiCmdBuf.Count > 0)
                    {
                        stuWholeCmdBuf mOneCmd = new stuWholeCmdBuf();
                        mOneCmd = lststu_ReceiCmdBuf[0];
                        lststu_ReceiCmdBuf.RemoveAt(0);
                        JObject job1 = mOneCmd.jobjOneContent;
                        string str1 = JsonConvert.SerializeObject(mOneCmd.jobjOneContent);

                        string str2 = "接收用户发送的信息";
                        for (int i = 0; i < Li_MoitClient.Count; i++)
                        {
                            str1 = Li_MoitClient[i];
                        }
                        frmMain.strMessageDispBuf = str2 + str1 + "\r\n";
                        Li_MoitClient = new List<string>();
                        gLogWriter.WriteLog(str2, str1);//日志
                        if ((job1.Property("seq") != null) && (job1.Property("cmdType") != null) && (job1.Property("cmdAction") != null) &&
                            (job1.Property("sender") != null) && (job1.Property("receiver") != null) && (job1.Property("paramList").Count > 0))
                        {
                            #region 系统_SYS
                            if ((job1["cmdType"].ToString() == "1"))
                            {
                                string strName = job1["sender"].ToString();
                                #region 登录
                                if ((job1["cmdAction"].ToString() == "Login") && (job1["receiver"].ToString() == "CmdServer"))
                                {
                                    string Struser = job1["paramList"][0]["value"].ToString();//账号
                                    string Password = job1["paramList"][0]["param"].ToString();//密码
                                    string MoUser = job1["receiver"].ToString();//Struser用户发送到监控端的地址
                                    StrUserdizhi.Add(Struser);
                                    //2)check password is right//检测用户名密码是否正确(后由数据库获取)
                                 //   string strDatetime = DateTime.Now.ToString("yyyyMMddhhssfff");
                                    if ((Struser == "admin" || Struser == "lisi" || Struser == "zhangsan" || Struser == "Client") && (Password == "admin123"))
                                    {
                                        //3)check user is aready login ,if login force other loginout //检验登录的
                                        if ((frmMain.g_myServer.g_lstConnectClientTab.Count == 1) && (frmMain.g_myServer.g_lstConnectClientTab[0].strLoginUserName == ""))//给连接用户赋值名字
                                        {
                                            frmMain.g_myServer.g_lstConnectClientTab[0].strLoginUserName = Struser;//赋用户名
                                        }
                                        else if ((frmMain.g_myServer.g_lstConnectClientTab.Count == 2) && (frmMain.g_myServer.g_lstConnectClientTab[1].strLoginUserName == ""))
                                        {
                                            frmMain.g_myServer.g_lstConnectClientTab[1].strLoginUserName = Struser;
                                            LoadUserUpdateByConnectImf(frmMain.g_myServer.g_lstConnectClientTab[1].ipaEndIP.ToString(), frmMain.g_myServer.g_lstConnectClientTab[1].intEndPort, Struser);
                                        }
                                        else if ((frmMain.g_myServer.g_lstConnectClientTab.Count == 3) && (frmMain.g_myServer.g_lstConnectClientTab[2].strLoginUserName == ""))
                                        {
                                            frmMain.g_myServer.g_lstConnectClientTab[2].strLoginUserName = Struser;
                                            LoadUserUpdateByConnectImf(frmMain.g_myServer.g_lstConnectClientTab[2].ipaEndIP.ToString(), frmMain.g_myServer.g_lstConnectClientTab[0].intEndPort, Struser);
                                        }
                                        else if ((frmMain.g_myServer.g_lstConnectClientTab.Count == 4) && (frmMain.g_myServer.g_lstConnectClientTab[3].strLoginUserName == ""))
                                        {
                                            frmMain.g_myServer.g_lstConnectClientTab[3].strLoginUserName = Struser;
                                            LoadUserUpdateByConnectImf(frmMain.g_myServer.g_lstConnectClientTab[3].ipaEndIP.ToString(), frmMain.g_myServer.g_lstConnectClientTab[3].intEndPort, Struser);
                                        }
                                        CheckStaus(Struser);
                                        string str = "登录服务器成功";
                                        Cmd_IRCamPaletteOpt_OK(str, Struser);//登录成功
                                    }
                                    else
                                    {
                                        CheckStaus(Struser);
                                        string str = "账号或密码输入错误，请重新输入！！！";
                                        Cmd_IRCamPaletteOpt_OK(str, Struser);//登录成功
                                    }
                                }
                                #endregion
                                #region 用户下线
                                else if (job1["cmdAction"].ToString() == "Logout")
                                {
                                    string username = job1["paramList"][0]["username"].ToString();//获取需要下线的用户名
                                    CheckStaus(username);
                                    for (int i = 0; i < frmMain.g_myServer.g_lstConnectClientTab.Count; i++)
                                    {
                                        if (frmMain.g_myServer.g_lstConnectClientTab[i].strLoginUserName == username)
                                        {
                                            Cmd_IRCamPaletteOpt_OUT(username);
                                            string str6 = frmMain.g_myServer.g_lstConnectClientTab[i].ipaEndIP.ToString();
                                            string str7 = frmMain.g_myServer.g_lstConnectClientTab[i].intEndPort.ToString();
                                            int int7 = Convert.ToInt32(str7);
                                            frmMain.mGuiSysOppt.funForceDisOneUserConnect(str6, int7, username);
                                        }
                                    }
                                }
                                #endregion
                                #region 心跳
                                else if (job1["cmdAction"].ToString() == "Palpitate")
                                {

                                    string strUser = job1["sender"].ToString();
                                    CheckStaus(strName);
                                    for (int i = 0; i < StrUserdizhi.Count; i++)
                                    {
                                        string struser = StrUserdizhi[i];//
                                        string str = "心跳成功";
                                        Cmd_IRCamPalpitate_OK(str, struser);//返回心跳命令s
                                    }
                                }
                                #endregion
                                #region 启停止巡检 
                                //启停止巡检_CruiseSet//查询巡检状态_CruiseStateGet//状态查询_ObjStateGet//监控头重启_MonDevRestart
                                else if ((job1["cmdAction"].ToString() == "CruiseSet") || (job1["cmdAction"].ToString() == "CruiseStateGet")
                                    || (job1["cmdAction"].ToString() == "ObjStateGet") || (job1["cmdAction"].ToString() == "MonDevRestart"))
                                {
                                    string strDizhi = job1["sender"].ToString();
                                    string MoUser = job1["receiver"].ToString();//获取命令转发的用户名
                                    for (int j = 0; j < frmMain.C_myServer.g_lstConnectMonitorTab.Count; j++)
                                    {
                                        string strdi = frmMain.C_myServer.g_lstConnectMonitorTab[j].strLoginUserName;//获取监控地址
                                        if (strdi == MoUser)//判断命令中的地址是否存在于在线的客户端
                                        {
                                            lstintUserOut = 1;//1表示地址存在,0表示不存在
                                            string strjson = JsonConvert.SerializeObject(job1);//获取命令
                                            lststu_ReceiStrCmd.Add(strjson);
                                            frmMain.m_MOJsonCtrl.funSendOneFramCmd(MoUser, strjson);//转发
                                            lststu_ReceiStrCmd = new List<string>();//清空
                                        }
                                    }
                                    if (lstintUserOut == 1)
                                    {
                                        lstintUserOut = 0;
                                    }
                                    else
                                    {
                                        string str = "你选择地址不在线！！！";
                                        Cmd_IRCamPalpitate_OK(str, strDizhi);//返回地址不存在命令
                                    }
                                }
                                #endregion
                            }
                            #endregion
                            #region 云台
                            else if (job1["cmdType"].ToString() == "2")
                            {
                                //云台转动_PTZMove//云台角度_PTZMoveAngleSet//云台设置预置位_PrePosSet//云台调用预置位_PrePosInvoke//查云台角度_PTZAngleGet//云台初始化_PTZInit
                                if ((job1["cmdAction"].ToString() == "PTZMove") || (job1["cmdAction"].ToString() == "PTZMoveAngleSet")
                                    || (job1["cmdAction"].ToString() == "PrePosSet") || (job1["cmdAction"].ToString() == "PrePosInvoke")
                                    || (job1["cmdAction"].ToString() == "PTZAngleGet") || (job1["cmdAction"].ToString() == "PTZInit"))
                                {
                                    string strDizhi = job1["sender"].ToString();
                                    string MoUser = job1["receiver"].ToString();//获取命令转发的用户名
                                    for (int j = 0; j < frmMain.C_myServer.g_lstConnectMonitorTab.Count; j++)
                                    {
                                        string strdi = frmMain.C_myServer.g_lstConnectMonitorTab[j].strLoginUserName;//获取监控地址
                                        if (strdi == MoUser)//判断命令中的地址是否存在于在线的客户端
                                        {
                                            lstintUserOut = 1;//1表示地址存在,0表示不存在
                                            string strjson = JsonConvert.SerializeObject(job1);//获取命令
                                            //  lststu_ReceiStrCmd.Add(strjson);
                                            frmMain.m_MOJsonCtrl.funSendOneFramCmd(MoUser, strjson);//转发
                                            lststu_ReceiStrCmd = new List<string>();//清空
                                        }
                                    }
                                    if (lstintUserOut == 1)
                                    {
                                        lstintUserOut = 0;
                                    }
                                    else
                                    {
                                        string str = "你选择地址不在线！！！";
                                        Cmd_IRCamPalpitate_OK(str, strDizhi);//返回地址不存在命令
                                    }
                                }
                            }
                            #endregion
                            #region 可见光
                            else if (job1["cmdType"].ToString() == "3")
                            {
                                //手动变焦_ZoomOpt//直接变焦位置_ZoomPosSet//变焦位置查询_ZoomPosGet//存图_SaveImg
                                if ((job1["cmdAction"].ToString() == "ZoomOpt") || (job1["cmdAction"].ToString() == "ZoomPosSet")
                                    || (job1["cmdAction"].ToString() == "ZoomPosGet") || (job1["cmdAction"].ToString() == "SaveImg"))
                                {
                                    string strDizhi = job1["sender"].ToString();
                                    string MoUser = job1["receiver"].ToString();//获取命令转发的用户名
                                    for (int j = 0; j < frmMain.C_myServer.g_lstConnectMonitorTab.Count; j++)
                                    {
                                        string strdi = frmMain.C_myServer.g_lstConnectMonitorTab[j].strLoginUserName;//获取监控地址
                                        if (strdi == MoUser)//判断命令中的地址是否存在于在线的客户端
                                        {
                                            lstintUserOut = 1;//1表示地址存在,0表示不存在
                                            string strjson = JsonConvert.SerializeObject(job1);//获取命令
                                            lststu_ReceiStrCmd.Add(strjson);
                                            frmMain.m_MOJsonCtrl.funSendOneFramCmd(MoUser, strjson);//转发
                                            lststu_ReceiStrCmd = new List<string>();//清空
                                        }
                                    }
                                    if (lstintUserOut == 1)
                                    {
                                        lstintUserOut = 0;
                                    }
                                    else
                                    {
                                        string str = "你选择地址不在线！！！";
                                        Cmd_IRCamPalpitate_OK(str, strDizhi);//返回地址不存在命令
                                    }
                                }
                            }
                            #endregion
                            #region 红外控制
                            else if (job1["cmdType"].ToString() == "4")
                            {
                                //手动聚焦_ManualFocus//自动聚焦_AutoFocus//直接聚焦位置_FocusPosSet/聚焦位置查询_FocusPosGet/色板设置_PaletteSet//数字变焦设置_DigZoomSet
                                //数字变焦获取_DigZoomGet//调节模式_AdjustModeSet//手动调节_ManualAdjustSet//红外热图保存_SaveIRHotImg//存视频图_SaveVideoImg
                                if ((job1["cmdAction"].ToString() == "ManualFocus") || (job1["cmdAction"].ToString() == "AutoFocus")
                                  || (job1["cmdAction"].ToString() == "FocusPosSet") || (job1["cmdAction"].ToString() == "FocusPosGet")
                                    || (job1["cmdAction"].ToString() == "PaletteSet") || (job1["cmdAction"].ToString() == "DigZoomSet")
                                    || (job1["cmdAction"].ToString() == "DigZoomGet") || (job1["cmdAction"].ToString() == "AdjustModeSet")
                                    || (job1["cmdAction"].ToString() == "ManualAdjustSet") || (job1["cmdAction"].ToString() == "SaveIRHotImg")
                                    || (job1["cmdAction"].ToString() == "SaveVideoImg"))
                                {
                                    string strDizhi = job1["sender"].ToString();
                                    string MoUser = job1["receiver"].ToString();//获取命令转发的用户名
                                    for (int j = 0; j < frmMain.C_myServer.g_lstConnectMonitorTab.Count; j++)
                                    {
                                        string strdi = frmMain.C_myServer.g_lstConnectMonitorTab[j].strLoginUserName;//获取监控地址
                                        if (strdi == MoUser)//判断命令中的地址是否存在于在线的客户端
                                        {
                                            lstintUserOut = 1;//1表示地址存在,0表示不存在
                                            string strjson = JsonConvert.SerializeObject(job1);//获取命令
                                            lststu_ReceiStrCmd.Add(strjson);
                                            frmMain.m_MOJsonCtrl.funSendOneFramCmd(MoUser, strjson);//转发
                                            lststu_ReceiStrCmd = new List<string>();//清空
                                        }
                                    }
                                    if (lstintUserOut == 1)
                                    {
                                        lstintUserOut = 0;
                                    }
                                    else
                                    {
                                        string str = "你选择地址不在线！！！";
                                        Cmd_IRCamPalpitate_OK(str, strDizhi);//返回地址不存在命令
                                    }
                                }
                            }
                            #endregion
                            #region 红外分析
                            else if (job1["cmdType"].ToString() == "5")
                            {
                                #region
                                //辐射率设置EmissivitySet/辐射率读取EmissivityGet/反射温度设置RefTempSet/反射温度读取RefTempGet/空气温度设置AirTempSet/空气温度读取AirTempGet/空气湿度设置AirHumSet
                                //空气湿度读取AirHumGet/距离设置DistanceSet/距离读取DistanceGet/窗口温度设置WinTempSet /窗口温度读取WinTempGet/窗口穿透率设置WinTrmRateSet/窗口穿透率读取WinTrmRateGet
                                //查当前测温状态AnaStateGet/清除所有测温点AnaClearAll/点温位置设置AnaSpotPosSet/点温位置读取AnaSpotPosGet/点温参数设置AnaSpotParamSet/点温参数读取AnaSpotParamGet
                                //点温度获取AnaSpotTempGet/线温位置设置AnaLinePosSet/线温位置读取AnaLinePosGet/线温参数设置AnaLineParamSet/线温参数读取AnaLineParamGet/线温获取AnaLineTempGet/矩形测温位置设置AnaAreaPosSet
                                //矩形测温位置读取AnaAreaPosGet/矩形测温参数设置AnaAreaParamSet/矩形温参数获取AnaAreaParamGet/矩形测温温度获取AnaAreaTempGet/多边形温位置设置AnaPolyPosSet/多边形温位置读取AnaPolyPosGet
                                //多边形温参数设置AnaPolyParamSet/多边形温参数获取AnaPolyParamGet/多边形测温温度获取AnaPolyTempGet/圆形温位置设置AnaCirclePosSet/读圆形温位置AnaCirclePosGet/圆形温参数AnaCircleParamSet
                                //读圆形温参数AnaCircleParamGet/读圆形温AnaCircleTempGet
                                #endregion
                                if ((job1["cmdAction"].ToString() == "EmissivitySet") || (job1["cmdAction"].ToString() == "EmissivityGet")
                                 || (job1["cmdAction"].ToString() == "RefTempSet") || (job1["cmdAction"].ToString() == "RefTempGet")
                                   || (job1["cmdAction"].ToString() == "AirTempSet") || (job1["cmdAction"].ToString() == "AirTempGet")
                                   || (job1["cmdAction"].ToString() == "AirHumSet") || (job1["cmdAction"].ToString() == "AdjustModeSet")
                                   || (job1["cmdAction"].ToString() == "ManualAdjustSet") || (job1["cmdAction"].ToString() == "AirHumGet")
                                   || (job1["cmdAction"].ToString() == "DistanceSet") || (job1["cmdAction"].ToString() == "DistanceGet")
                                    || (job1["cmdAction"].ToString() == "WinTempSet") || (job1["cmdAction"].ToString() == "WinTempGet")
                                    || (job1["cmdAction"].ToString() == "WinTrmRateSet") || (job1["cmdAction"].ToString() == "WinTrmRateGet")
                                    || (job1["cmdAction"].ToString() == "AnaStateGet") || (job1["cmdAction"].ToString() == "AnaClearAll")
                                    || (job1["cmdAction"].ToString() == "AnaSpotPosSet") || (job1["cmdAction"].ToString() == "AnaSpotPosGet")
                                    || (job1["cmdAction"].ToString() == "AnaSpotParamSet") || (job1["cmdAction"].ToString() == "AnaSpotParamGet")
                                    || (job1["cmdAction"].ToString() == "AnaSpotTempGet") || (job1["cmdAction"].ToString() == "AnaLinePosSet")
                                    || (job1["cmdAction"].ToString() == "AnaLinePosGet") || (job1["cmdAction"].ToString() == "AnaLineParamSet")
                                    || (job1["cmdAction"].ToString() == "AnaLineTempGet")
                                    || (job1["cmdAction"].ToString() == "AnaLineParamGet") || (job1["cmdAction"].ToString() == "AnaAreaPosSet")
                                    || (job1["cmdAction"].ToString() == "AnaAreaPosGet") || (job1["cmdAction"].ToString() == "AnaAreaParamSet")
                                    || (job1["cmdAction"].ToString() == "AnaAreaParamGet") || (job1["cmdAction"].ToString() == "AnaAreaTempGet")
                                    || (job1["cmdAction"].ToString() == "AnaPolyPosSet") || (job1["cmdAction"].ToString() == "AnaPolyPosGet")
                                    || (job1["cmdAction"].ToString() == "AnaPolyParamSet") || (job1["cmdAction"].ToString() == "AnaPolyParamGet")
                                    || (job1["cmdAction"].ToString() == "AnaPolyTempGet") || (job1["cmdAction"].ToString() == "AnaCirclePosSet")
                                    || (job1["cmdAction"].ToString() == "AnaCirclePosGet") || (job1["cmdAction"].ToString() == "AnaCircleParamSet")
                                    || (job1["cmdAction"].ToString() == "AnaCircleParamGet") || (job1["cmdAction"].ToString() == "AnaCircleTempGet"))
                                {
                                    string strDizhi = job1["sender"].ToString();
                                    string MoUser = job1["receiver"].ToString();//获取命令转发的用户名
                                    for (int j = 0; j < frmMain.C_myServer.g_lstConnectMonitorTab.Count; j++)
                                    {
                                        string strdi = frmMain.C_myServer.g_lstConnectMonitorTab[j].strLoginUserName;//获取监控地址
                                        if (strdi == MoUser)//判断命令中的地址是否存在于在线的客户端
                                        {
                                            lstintUserOut = 1;//1表示地址存在,0表示不存在
                                            string strjson = JsonConvert.SerializeObject(job1);//获取命令
                                            lststu_ReceiStrCmd.Add(strjson);
                                            frmMain.m_MOJsonCtrl.funSendOneFramCmd(MoUser, strjson);//转发
                                            lststu_ReceiStrCmd = new List<string>();//清空
                                        }
                                    }
                                    if (lstintUserOut == 1)
                                    {
                                        lstintUserOut = 0;
                                    }
                                    else
                                    {
                                        string str = "你选择地址不在线！！！";
                                        Cmd_IRCamPalpitate_OK(str, strDizhi);//返回地址不存在命令
                                    }
                                }
                            }
                            #endregion
                            #region 温湿度
                            else if (job1["cmdType"].ToString() == "6")
                            {
                                if (job1["cmdAction"].ToString() == "TempHumGet")
                                {
                                    string strDizhi = job1["sender"].ToString();
                                    string MoUser = job1["receiver"].ToString();//获取命令转发的用户名
                                    for (int j = 0; j < frmMain.C_myServer.g_lstConnectMonitorTab.Count; j++)
                                    {
                                        string strdi = frmMain.C_myServer.g_lstConnectMonitorTab[j].strLoginUserName;//获取监控地址
                                        if (strdi == MoUser)//判断命令中的地址是否存在于在线的客户端
                                        {
                                            lstintUserOut = 1;//1表示地址存在,0表示不存在
                                            string strjson = JsonConvert.SerializeObject(job1);//获取命令
                                            lststu_ReceiStrCmd.Add(strjson);
                                            frmMain.m_MOJsonCtrl.funSendOneFramCmd(MoUser, strjson);//转发
                                            lststu_ReceiStrCmd = new List<string>();//清空
                                        }
                                    }
                                    if (lstintUserOut == 1)
                                    {
                                        lstintUserOut = 0;
                                    }
                                    else
                                    {
                                        string str = "你选择地址不在线！！！";
                                        Cmd_IRCamPalpitate_OK(str, strDizhi);//返回地址不存在命令
                                    }
                                }
                            }
                            #endregion
                        }
                        return;
                    }

                    else
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return;
        }


        public void funSendClient(int intLstIndex, string strbuf)
        {
            string str1;
            int intre = -1;
            try
            {
                str1 = JsonConvert.SerializeObject(strbuf);
                stuBaseNumFrame stuBf = new stuBaseNumFrame();
                stuBf = funOneContentToBaseFrame(strbuf);
                byte[] btbuf;
                btbuf = funOneBaseNumFrameToByteArray(stuBf);

                if ((intLstIndex >= 0) && (intLstIndex < frmMain.g_myServer.g_lstConnectClientTab.Count))
                {
                    intre = frmMain.g_myServer.g_lstConnectClientTab[intLstIndex].sendMegToCilent(btbuf);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>发送
        /// <param name="strTo">用户名//地址</param>
        /// <param name="strbuf">命令</param>
        public void funSendOneFramCmd(string strTo, string strbuf)
        {
            string str1;
            try
            {
                str1 = JsonConvert.SerializeObject(strbuf);
                stuBaseNumFrame stuBf = new stuBaseNumFrame();
                stuBf = funOneContentToBaseFrame(strbuf);
                byte[] btbuf;
                btbuf = funOneBaseNumFrameToByteArray(stuBf);
                for (int i = 0; i < frmMain.g_myServer.g_lstConnectClientTab.Count; i++)
                {
                    if ((strTo == frmMain.g_myServer.g_lstConnectClientTab[i].strLoginUserName))
                    {
                        frmMain.g_myServer.SendOneClientMsgByIndex(i, btbuf);
                    }
                }

            }
            catch (Exception)
            {
            }
        }
        public void funSendOneFramCmd_repeat(string strTo, string strbuf)//异地登录
        {
            string str1;
            try
            {
                str1 = JsonConvert.SerializeObject(strbuf);
                stuBaseNumFrame stuBf = new stuBaseNumFrame();
                stuBf = funOneContentToBaseFrame(strbuf);
                byte[] btbuf;
                btbuf = funOneBaseNumFrameToByteArray(stuBf);
                for (int i = 0; i < frmMain.g_myServer.g_lstConnectClientTab.Count; i++)
                {
                    if ((strTo == frmMain.g_myServer.g_lstConnectClientTab[i].strLoginUserName))
                    {
                        frmMain.g_myServer.SendOneClientMsgByIndex(i, btbuf);
                    }
                    return;
                }

            }
            catch (Exception)
            {
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="strTo"></param>
        ///// <param name="strbuf"></param>
        //public void funSendOneFramCmd_Monito(string strClDizhi, string strbuf)
        //{
        //    string str1;
        //    try
        //    {
        //        str1 = JsonConvert.SerializeObject(strbuf);
        //        stuBaseNumFrame stuBf = new stuBaseNumFrame();
        //        stuBf = funOneContentToBaseFrame(strbuf);
        //        byte[] btbuf;
        //        btbuf = funOneBaseNumFrameToByteArray(stuBf);

        //        for (int i = 0; i < frmMain.g_myServer.g_lstConnectClientTab.Count; i++)
        //        {
        //            if (strClDizhi == frmMain.g_myServer.g_lstConnectClientTab[i].strLoginUserDizhi)
        //            {
        //                frmMain.g_myServer.SendOneClientMsgByIndex(i, btbuf);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}
        public byte[] funOneBaseNumFrameToByteArray(stuBaseNumFrame m_stuOneBaseNumFramBuf)   //一帧数据转发化字节数组
        {
            int i, intCrc;
            intCrc = 0x00;
            int intlen = m_stuOneBaseNumFramBuf.bt_Content.Length;// byteArray.Length;
            byte[] btRe = new byte[intlen + 12];
            //给字头
            btRe[0] = m_stuOneBaseNumFramBuf.bt2_SOI[0];
            btRe[1] = m_stuOneBaseNumFramBuf.bt2_SOI[1];
            //给随机码
            btRe[2] = m_stuOneBaseNumFramBuf.bt_RandCode;
            intCrc = intCrc ^ btRe[2];
            //给SID
            for (i = 0; i < 4; i++)
            {
                btRe[3 + i] = m_stuOneBaseNumFramBuf.bt4_SID[i];
                // btRe[3 + i] = 0x00;
                intCrc = intCrc ^ btRe[3 + i];
            }
            //////给长度
            btRe[7] = (byte)(intlen % 256);
            btRe[8] = (byte)(intlen / 256);
            btRe[9] = 0x00;
            btRe[10] = 0x00;
            //给校验

            intCrc = intCrc ^ btRe[7];
            intCrc = intCrc ^ btRe[8];
            intCrc = intCrc ^ btRe[9];
            intCrc = intCrc ^ btRe[10];

            /////给内容
            for (i = 0; i < intlen; i++)
            {
                // btRe[11 + i] =  m_stuOneBaseNumFramBuf.bt_Content[i];// byteArray[i];
                btRe[11 + i] = (byte)(m_stuOneBaseNumFramBuf.bt_Content[i] ^ m_stuOneBaseNumFramBuf.bt_RandCode);// byteArray[i];
                intCrc = intCrc ^ btRe[11 + i];
            }
            //给校验
            btRe[intlen + 11] = (byte)(intCrc);

            return btRe;
        }
        public stuBaseNumFrame funOneContentToBaseFrame(string jobjBuf)  //一个Josn帧转发为基础帧
        {
            int i;
            stuBaseNumFrame stuRe_BaseNumeFram = new stuBaseNumFrame();
            stuRe_BaseNumeFram.bt2_SOI = new byte[2];
            stuRe_BaseNumeFram.bt2_SOI[0] = m_stuSystemVar.bt2_SOI[0];// 0xA5;
            stuRe_BaseNumeFram.bt2_SOI[1] = m_stuSystemVar.bt2_SOI[1];// 0x5A;

            Random rd = new Random();
            i = (int)(rd.Next() % 256);
            stuRe_BaseNumeFram.bt_RandCode = (byte)i;
            stuRe_BaseNumeFram.bt4_SID = new byte[4];
            for (i = 0; i < 4; i++)
            {
                stuRe_BaseNumeFram.bt4_SID[i] = m_stuSystemVar.bt4_SID[i];
            }
            string strContentBuf = "";
            strContentBuf = JsonConvert.SerializeObject(jobjBuf);
            stuRe_BaseNumeFram.bt_Content = System.Text.Encoding.UTF8.GetBytes(strContentBuf);
            return stuRe_BaseNumeFram;
        }
        public void funWaitForResult(bool blClearReceiBufFirst, int intMaxWaitTimeMs)    //发命令等待返回函数
        {
            if (blClearReceiBufFirst)
            {
                m_lststuReturn = new List<stuReceiReturnCmdFormat>();
                m_lst_JobjReturnCmdBuf = new List<JObject>();
            }
            DateTime dt1 = DateTime.Now;
            while (true)
            {
                Application.DoEvents();
                Thread.Sleep(50);
                if (m_lststuReturn.Count > 0)
                {
                    break;
                }
                if (m_lst_JobjReturnCmdBuf.Count > 0)
                {
                    break;
                }
                TimeSpan ts = DateTime.Now - dt1;
                if ((ts.TotalMilliseconds > intMaxWaitTimeMs) || (ts.TotalMilliseconds > 5000))
                {
                    break;
                }
            }
        }
        #region 内部操作
        public void myServer_OnRecvMsg(object source, Server.EventArgs_Recv e)//解析命令
        {
            byte btbuf;
            for (int i = 0; i < e.btRecevByte.Length; i++)
            {
                btbuf = e.btRecevByte[i];
                frmMain.m_ClJsonCtrl.funRecieveOneByteToBuf(btbuf);
            }
        }
        public void m_clsClient_OnConnect(object obj, Server.EventArgs_Connect e)//连接状态
        {

            if (m_stuSystemVar.intLoginStatus > 0)
            {
                m_stuSystemVar.intConnectStatus = 2;
                m_stuSystemVar.intLoginStatus = 0;
            }
            else
            {
                m_stuSystemVar.intConnectStatus = 1;
            }
        }
        public void OptJsonConnectServer()
        {
            frmMain.g_myServer.OnConnect += new Server.EventHandler_Connect(m_clsClient_OnConnect);//连接登录状态
            frmMain.g_myServer.OnRecvMsg += new Server.EventHandler_Recv(myServer_OnRecvMsg);//接收
            frmMain.g_myServer.StartServer();//启动服务器
        }

        #endregion
    }
}
