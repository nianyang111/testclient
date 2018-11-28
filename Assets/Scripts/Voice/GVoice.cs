using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using gcloud_voice;

public class GVoice : MonoBehaviour {

    private IGCloudVoice m_voiceengine = null;
    private string m_recordpath;
    private string m_downloadpath;
    private static string m_fileid = "";
    private byte[] m_ShareFileID = null; /*when send record file save in svr, we will return a fileid in OnSendFileComplete callback function, you can save it ,and download  record by this fileid*/
    private static GVoice instance = null;

    public CallBack<string> strCall;
    public static GVoice Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject();
                instance = go.AddComponent<GVoice>();
                go.name = instance.GetType().ToString();
                DontDestroyOnLoad(go);

            }
            return instance;
        }
        set { instance = value; }
    }

    void Awake()
    {
        if (m_voiceengine == null && Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_voiceengine = GCloudVoice.GetEngine();
            System.TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string strTime = System.Convert.ToInt64(ts.TotalSeconds).ToString();
            //m_voiceengine.SetAppInfo("932849489","d94749efe9fce61333121de84123ef9b",strTime);
            var info = m_voiceengine.SetAppInfo("1753605946", "445aec6d7d557ace513bc45ec3adc77e", strTime);
            m_voiceengine.SetServerInfo("udp://cn.voice.gcloudcs.com:10001");
            
            m_voiceengine.Init();
            m_voiceengine.SetMode(GCloudVoiceMode.Messages);
            m_voiceengine.OnApplyMessageKeyComplete += (IGCloudVoice.GCloudVoiceCompleteCode code) =>
            {
                PlayLog("OnApplyMessageKeyComplete c# callback");
                if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_MESSAGE_KEY_APPLIED_SUCC)
                {
                    PlayLog("OnApplyMessageKeyComplete succ11");
                }
                else
                {
                    PlayLog("OnApplyMessageKeyComplete error");
                }
            };
            m_voiceengine.OnUploadReccordFileComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid) =>
            {
                PlayLog("OnUploadReccordFileComplete c# callback");
                if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_UPLOAD_RECORD_DONE)
                {
                    m_fileid = fileid;
                    if (upLoadScuccessCall != null)
                        upLoadScuccessCall(m_fileid);
                    //UserTalk talk = new UserTalk();
                    //talk.type = 2;
                    //talk.msg = m_fileid;
                    //talk.subtype = 1;
                    //SocketClient.Instance.AddSendMessageQueue(new C2GMessage
                    //{
                    //    msgid = MessageId.C2G_UserTalk,
                    //    UserTalk = talk

                    //});
                    PlayLog("发送语音消息");
                    PlayLog("OnUploadReccordFileComplete succ, filepath:" + filepath + " fileid len=" + fileid.Length + " fileid:" + fileid + " fileid len=" + fileid.Length);
                }
                else
                {
                    PlayLog("OnUploadReccordFileComplete error");
                }
                
            };
            m_voiceengine.OnDownloadRecordFileComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string filepath, string fileid) =>
            {
                PlayLog("OnDownloadRecordFileComplete c# callback");
                if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_DOWNLOAD_RECORD_DONE)
                {
                    PlayLog("OnDownloadRecordFileComplete succ, filepath:" + filepath + " fileid:" + fileid);
                    if (downCall != null)
                    {
                        downCall();
                        downCall = null;
                    }
                    Click_btnPlayReocrdFile();
                }
                else
                {
                    PlayLog("OnDownloadRecordFileComplete error");
                }
            };
            m_voiceengine.OnPlayRecordFilComplete += (IGCloudVoice.GCloudVoiceCompleteCode code, string filepath) =>
            {
                PlayLog("OnPlayRecordFilComplete c# callback");
                if (code == IGCloudVoice.GCloudVoiceCompleteCode.GV_ON_PLAYFILE_DONE)
                {
                    PlayLog("OnPlayRecordFilComplete succ, filepath:" + filepath);
                    if (playCall != null)
                    {
                        playCall();
                        playCall = null;
                    }
                }
                else
                {
                    PlayLog("OnPlayRecordFilComplete error");
                }
            };
        }
        m_recordpath = Application.persistentDataPath + "/" + "recording.dat";
        m_downloadpath = Application.persistentDataPath + "/" + "download.dat";
    }
    void Update()
    {
        //PlayLog("update...");
        if (m_voiceengine == null)
        {
            PlayLog("m_voiceengine is null");
        }
        else
        {
            m_voiceengine.Poll();
        }
    }
    public void Click_btnReqAuthKey()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            PlayLog("ApplyMessageKey btn click");
            m_voiceengine.ApplyMessageKey(15000);
        }
    }

    float music_beforeStart;
    float sound_beforeStart;
    public void Click_btnStartRecord()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            music_beforeStart = AudioManager.Instance.MusicValue;
            sound_beforeStart = AudioManager.Instance.SoundValue;
            AudioManager.Instance.SoundValue = 0;
            AudioManager.Instance.MusicValue = 0;
            PlayLog("startrecord btn click, recordpath=" + m_recordpath);
            m_voiceengine.StartRecording(m_recordpath);
        }
    }
    public void Click_btnStopRecord()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            AudioManager.Instance.SoundValue = sound_beforeStart;
            AudioManager.Instance.MusicValue = music_beforeStart;
            PlayLog("stoprecord btn click");
            m_voiceengine.StopRecording();
        }
    }

    private CallBack<string> upLoadScuccessCall;
    public void Click_btnUploadFile(CallBack<string> upLoadScuccessCall=null)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            this.upLoadScuccessCall = upLoadScuccessCall;
            int ret1 = m_voiceengine.UploadRecordedFile(m_recordpath, 60000);
            PlayLog("Click_btnUploadFile file with ret==" + ret1);
        }
    }


    private CallBack downCall;

    public void Click_btnDownloadFile(string fileid=null,CallBack _downCall=null,CallBack _playCall=null)
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            downCall = _downCall;
            playCall = _playCall;
            //GcloudVoice.GcloudVoiceErrno err;
            //err = m_voiceengine.DownRecordFile (m_ShareFileID, m_downloadpath, 5000);
            //PrintLog ("download file with ret=" + err);
            //s_strLog += "\r\n download file with ret=="+err;
            //m_fileid = "306b02010004643062020100042433343664633236302d613163302d313165362d386264392d66353435376438313232353602037a1afd02047d16a3b402045826b397042033363236333939656531366162666333396561376439613238373432383135380201000201000400";
            if (!string.IsNullOrEmpty(fileid))
            {
                m_fileid = fileid;
            }
            int ret = m_voiceengine.DownloadRecordedFile(m_fileid, m_downloadpath, 60000);
        }
    }

    private CallBack playCall = null;

    public void Click_btnPlayReocrdFile()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int err;
            if (false && m_ShareFileID == null)
            {
                //UnityEditor.EditorUtility.DisplayDialog("", "you have not download record file ,we will play local record files", "OK");
                err = m_voiceengine.PlayRecordedFile(m_recordpath);
                return;
            }
            err = m_voiceengine.PlayRecordedFile(m_downloadpath);
            //m_voiceengine.PlayRecordedFile (m_downloadpath);
        }
    }
    public void Click_btnStopPlayRecordFile()
    {
        //GcloudVoice.GcloudVoiceErrno err;
        //err = m_voiceengine.StopPlayFile ();
        //PrintLog ("stopplay file with ret=" + err);
        //m_voiceengine.StopPlayFile ();
        m_fileid = "306b02010004643062020100042433343664633236302d613163302d313165362d386264392d66353435376438313232353602037a1afd02047d16a3b402045826b397042033363236333939656531366162666333396561376439613238373432383135380201000201000400";
        m_fileid = "306b02010004643062020100042433343664633236302d613163302d313165362d386264392d66353435376438313232353602037a1afd02041316a3b4020458298a28042033626433653134643866343434613666313537356135383338313566383563340201000201000400";
        int ret = m_voiceengine.DownloadRecordedFile(m_fileid, m_downloadpath, 60000);
    }
    public void Click_GetRecFileParam()
    {
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            int[] bytes = new int[1];
            bytes[0] = 0;
            float[] seconds = new float[1];
            seconds[0] = 0;
            m_voiceengine.GetFileParam(m_recordpath, bytes, seconds);
            PlayLog("\r\nfile:" + m_recordpath + "bytes:" + bytes[0] + " seconds:" + seconds[0]);
        }
    }

    void PlayLog(string str)
    {
        if (strCall != null)
            strCall(str);
    }
}
