﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using LitJson;
using EaseMob;
using UnityEngine.SceneManagement;

public class MainScene : MonoBehaviour {

	public Button friendListBtn;
	public InputField txtContent;
	public Button sendTxtMessageBtn;
	public Button sendGroupTxtMsgBtn;
	public RectTransform logContent;
	public Text logText;
	public Dropdown friendListDd;
	public Button screenShot;
	public InputField fromUser;
	public InputField msgId;
	public Button getMessageBtn;
	public Button getLastMsgBtn;
	public Button logoutBtn;
	public RawImage rawImage;
	public InputField filePath;
	public Button sendFileMessageBtn;
	public Button sendGroupFileMsgBtn;
	public InputField groupName;
	public InputField groupUser;
	public Button createGroupBtn;
	public Button leaveGroupBtn;
	public Button getGroupsBtn;
	public Button addToGroupBtn;
	public Button inviteToGroupBtn;
	public Button RmUserFromGroupBtn;
	public Button destroyGroupBtn;

	private List<Dropdown.OptionData> friendList = new List<Dropdown.OptionData>();
	private List<EMGroup> groupList = new List<EMGroup>();

	// Use this for initialization
	void Start () {
		
		rawImage.gameObject.SetActive(false);

		setMessageRecvListener ();

		setGroupListener ();


		friendListBtn.onClick.AddListener (delegate {
			friendList.Clear();
			friendListDd.ClearOptions();
			string names = EMClient.Instance.GetAllContactsFromServer();
			Dropdown.OptionData od = new Dropdown.OptionData();
			od.text = "choose";
			friendList.Add(od);
			if(names.Length > 0){
				string[] namearr = names.Split(new char[] { ',' });
				foreach(string name in namearr){
					Dropdown.OptionData dod = new Dropdown.OptionData();
					dod.text = name;
					friendList.Add(dod);
				}
			}
			friendListDd.AddOptions(friendList);
		});

		sendTxtMessageBtn.onClick.AddListener (delegate() {
			if(friendListDd.value == 0){
				logText.text = "not choose";
				logText.color = new Color(255,0,0);
				return;
			}
			Dropdown.OptionData dod = friendList[friendListDd.value];

			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "send message success";
			};
			cb.onProgressCallback = (progress,status) => {
				
			};
			cb.onErrorCallback = (code,message) => {
				
			};
			EMClient.Instance.SendTextMessage(txtContent.text, dod.text, ChatType.Chat, cb);
		});

		screenShot.onClick.AddListener (delegate() {
			StartCoroutine(GenCapture());
		});

		sendFileMessageBtn.onClick.AddListener (delegate() {
			if(filePath.text.Length > 0){
				Dropdown.OptionData dod = friendList[friendListDd.value];
				EMBaseCallback cb = new EMBaseCallback();
				cb.onSuccessCallback = () => {
					logText.text = "send file success";
				};
				cb.onProgressCallback = (progress,status) => {

				};
				cb.onErrorCallback = (code,msg) => {

				};
				EMClient.Instance.SendFileMessage(filePath.text,dod.text,ChatType.Chat,cb);
			}

		});

		sendGroupTxtMsgBtn.onClick.AddListener (delegate() {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "send group message success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,message) => {

			};
			if(txtContent.text.Length == 0)
			{
				txtContent.placeholder.GetComponent<Text>().text = "input here first";
				return;
			}
			if(groupName.text.Length == 0)
			{
				groupName.placeholder.GetComponent<Text>().text = "input here first";
				return;
			}
			EMClient.Instance.SendTextMessage(txtContent.text, groupName.text, ChatType.GroupChat, cb);

		});

		sendGroupFileMsgBtn.onClick.AddListener (delegate() {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "send group file success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,message) => {

			};

			if(groupName.text.Length == 0)
			{
				groupName.placeholder.GetComponent<Text>().text = "input here first";
				return;
			}
			EMClient.Instance.SendFileMessage(filePath.text,groupName.text,ChatType.GroupChat,cb);
		});

		createGroupBtn.onClick.AddListener (delegate () {
			if(groupName.text.Length > 0){
				EMGroupCallback cb  = new EMGroupCallback();
				cb.onSuccessCreateGroupCallback = (group) => {
					logText.text = "create suc. groupId="+group.mGroupId;
				};
				cb.onErrorCallback = (code,msg) => {

				};
				EMClient.Instance.createGroup(groupName.text,"desc:"+groupName.text,new string[0],"reason",200,GroupStyle.GroupStylePublicOpenJoin,cb);
			}
		});

		leaveGroupBtn.onClick.AddListener (delegate () {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "leave group success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,msg) => {

			};
			if(groupName.text.Length > 0)
				EMClient.Instance.leaveGroup(groupName.text,cb);
			else
				logText.text = "input group id first";
		});

		getGroupsBtn.onClick.AddListener (delegate() {
			logText.text = "";
			groupList.Clear();
			EMGroupCallback cb = new EMGroupCallback();
			cb.onSuccessGetGroupListCallback = (groups) => {
				foreach(EMGroup group in groups){
					logText.text += "ID="+group.mGroupId + "," + group.mGroupName + "\n";
					groupList.Add(group);
					groupName.text = group.mGroupId;
				}
				logContent.sizeDelta = new Vector2 (0, logText.preferredHeight+5);
			};
			cb.onErrorCallback = (code,msg) => {

			};
			EMClient.Instance.getJoinedGroupsFromServer(cb);
		});

		addToGroupBtn.onClick.AddListener (delegate() {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "add user to group success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,msg) => {

			};
			string[] users = {groupUser.text};
			EMClient.Instance.addUsersToGroup(groupName.text,users,cb);
		});

		inviteToGroupBtn.onClick.AddListener (delegate () {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "invite to group success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,msg) => {

			};
			if(groupName.text.Length > 0){
				string[] users = {groupUser.text};
				EMClient.Instance.inviteUser(groupName.text,users,"welconme",cb);
			}
			else
				logText.text = "input group id first";
		});


		RmUserFromGroupBtn.onClick.AddListener (delegate() {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "removeUserFromGroup success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,msg) => {

			};
			EMClient.Instance.removeUserFromGroup(groupName.text,groupUser.text,cb);
		});

		destroyGroupBtn.onClick.AddListener (delegate () {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				logText.text = "destory group success";
			};
			cb.onProgressCallback = (progress,status) => {

			};
			cb.onErrorCallback = (code,msg) => {

			};
			if(groupName.text.Length > 0)
				EMClient.Instance.destroyGroup(groupName.text,cb);
			else
				logText.text = "input group id first";
		});


		getLastMsgBtn.onClick.AddListener (delegate() {
			if(fromUser.text.Length == 0)
			{
				fromUser.placeholder.GetComponent<Text>().text = "input here first";
				return;
			}
			EMMessage message = EMClient.Instance.GetLatestMessage(fromUser.text);
			if(message != null){
				msgId.text = message.mMsgId;
				logText.text = message.mMsgId;
			}

		});

		getMessageBtn.onClick.AddListener (delegate() {
			if(fromUser.text.Length == 0)
			{
				fromUser.placeholder.GetComponent<Text>().text = "input here first";
				return;
			}
			if(msgId.text.Length == 0)
			{
				msgId.placeholder.GetComponent<Text>().text = "input here first";
				return;
			}

			logText.text = "";

			List<EMMessage> list = EMClient.Instance.GetConversationMessage(fromUser.text,msgId.text,20);
				foreach(EMMessage msg in list){
					logText.text += "msg id:"+msg.mMsgId+",from:"+msg.mFrom;
					if(msg.mType == MessageType.TXT)
						logText.text += ",txt:"+msg.mTxt;
					if(msg.mType == MessageType.FILE)
						logText.text += ",path:"+msg.mRemotePath;
					logText.text += "\n";

				logContent.sizeDelta = new Vector2 (0, logText.preferredHeight+5);
			}
		});


		logoutBtn.onClick.AddListener (delegate() {
			EMBaseCallback cb = new EMBaseCallback();
			cb.onSuccessCallback = () => {
				SceneManager.LoadScene("LoginScene");
			};
			cb.onProgressCallback = (progress,status) => {
				
			};
			cb.onErrorCallback = (code,message) => {
				
			};
			EMClient.Instance.Logout(true,cb);
		});
	}
			
	IEnumerator GenCapture()
	{
		yield return new WaitForEndOfFrame();

		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width,height,TextureFormat.RGB24,false);
		tex.ReadPixels(new Rect(0,0,width,height),0,0,true);
		byte[] imagebytes = tex.EncodeToPNG();//转化为png图
		tex.Compress(false);//对屏幕缓存进行压缩
		string imagePath = "";
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
			imagePath = Application.persistentDataPath;
		} else if (Application.platform == RuntimePlatform.WindowsPlayer) {
			imagePath = Application.dataPath;  
		} else if (Application.platform == RuntimePlatform.OSXEditor) {  
			imagePath = Application.dataPath; 
			imagePath = imagePath.Replace ("/Assets", null);  
		}

		string path = imagePath + "/" + "screencapture.png";
		File.WriteAllBytes(path,imagebytes);//存储png图
		logText.text = path;
		filePath.text = path;
	}



	private void setMessageRecvListener()
	{
		EMMessageListenerCallback receiveMessageCallback = new EMMessageListenerCallback();
		receiveMessageCallback.onMessageReceivedCallback = (msgs) =>
		{
			logText.text = "from ";
			foreach(EMMessage msg in msgs){
				logText.text += msg.mFrom;
				if(msg.mType == MessageType.TXT){
					logText.text += ",content="+msg.mTxt;
				}

				if(msg.mType == MessageType.IMAGE)
				{
					rawImage.gameObject.SetActive(true);
					rawImage.GetComponent<ShowPicture>().loadPic(msg.mRemotePath);
				}else{
					rawImage.gameObject.SetActive(false);
				}

				if(msg.mType == MessageType.VOICE || msg.mType == MessageType.FILE)
				{
					logText.text += msg.mRemotePath;

					EMBaseCallback cb = new EMBaseCallback();
					cb.onSuccessCallback = () => {
						logText.text = "recv file success:" + msg.mLocalPath;
					};
					cb.onProgressCallback = (progress,status) => {

					};
					cb.onErrorCallback = (code,msg1) => {
						logText.text = "recv file failure:" + msg.mLocalPath;
					};
					EMClient.Instance.DownloadAttachment(msg.mFrom, msg.mMsgId, cb);
				}
			}
		};
		receiveMessageCallback.onMessageReadAckReceivedCallback = (msgs) => 
		{

		};
		receiveMessageCallback.onMessageDeliveryAckReceivedCallback = (msgs) => 
		{

		};
		receiveMessageCallback.onMessageChangedCallback = (msgs) => 
		{

		};
		receiveMessageCallback.onCmdMessageReceivedCallback = (msgs) => 
		{

		};
		EMClient.Instance.receiveMessageCallback = receiveMessageCallback;
	}

	private void setGroupListener()
	{
		
		EMGroupListenerCallback groupListenerCallback = new EMGroupListenerCallback ();
		groupListenerCallback.onUserRemovedCallback = (groupId,groupName) => {
			logText.text = "received onUserRemovedCallback:[groupId="+groupId+",groupName="+groupName;
		};
		groupListenerCallback.onInvitationAccepted = (groupId, inviter, reason) => {
			logText.text = "received onInvitationAccepted:[groupId="+groupId+",inviter="+inviter+",reason="+reason;
		};
		groupListenerCallback.onInvitationDeclined = (groupId, invitee, reason) => {
			logText.text = "received onInvitationDeclined:[groupId="+groupId+",invitee="+invitee+",reason="+reason;
		};
		groupListenerCallback.onInvitationAccepted = (groupId, inviter, reason) => {
			logText.text = "received onInvitationAccepted:[groupId="+groupId+",inviter="+inviter+",reason="+reason;
		};
		groupListenerCallback.onGroupDestroyed = (groupId, groupName) => {
			logText.text = "received onGroupDestroyed:[groupId="+groupId+",groupName="+groupName;
		};
		groupListenerCallback.onAutoAcceptInvitationFromGroup = (groupId, inviter, inviteMessage) => {
			logText.text = "received onAutoAcceptInvitationFromGroup:[groupId="+groupId+",inviter="+inviter+",inviteMessage="+inviteMessage;
		};
		groupListenerCallback.onApplicationReceived = (groupId, groupName, applicant, reason) => {
			logText.text = "received onApplicationReceived:[groupId="+groupId+",groupName="+groupName+",applicant="+applicant+",reason="+reason;
		};
		groupListenerCallback.onApplicationDeclined = (groupId, groupName, decliner, reason) => {
			logText.text = "received onApplicationDeclined:[groupId="+groupId+",groupName="+groupName+",decliner="+decliner+",reason="+reason;
		};
		groupListenerCallback.onApplicationAccept = (groupId, groupName, accepter) => {
			logText.text = "received onApplicationAccept:[groupId="+groupId+",groupName="+groupName+",accepter="+accepter;
		};
		EMClient.Instance.groupListenerCallback = groupListenerCallback;

	}
}
