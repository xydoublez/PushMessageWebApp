﻿
using Messag.Utility.Config;

using Message.WebAPI.Services.IRepository;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Messag.Logger;

using Message.WebAPI.Controllers.Api;
using BusniessEntities.Models;
using Megadotnet.MessageMQ.Adapter;
using Message.WebAPI.Models;

namespace Message.WebAPI.Services.Repository
{
    public class MessageRepository:IMessageRepository
    {
        private static ILogger log = new Logger("MessageRepository");

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="messagemodel"></param>
        /// <returns></returns>
        public bool SendMessage(PushMessageModel messagemodel)
        {
          var activemq = new ActiveMQAdapter<PushMessageModel>(MQConfig.MQIpAddress, MQConfig.QueueDestination);
          return activemq.SendMessage<PushMessageModel>(messagemodel)>0;
        }


        private int GetUserIdListByUserAccount(string userAccount)
        {
            return 0;
        }


        /// <summary>
        /// Inserts the push message.
        /// </summary>
        /// <param name="pushmsg">The pushmsg.</param>
        /// <returns>成功插入PushMessage的PK的列表</returns>
        //[TransactionScopeCallHandler]
        public int[] InsertPushMessage(PushMsg pushmsg)
        {

            IList<T_BD_PushMessage> pushMessageDtoList = new List<T_BD_PushMessage>();
            pushmsg.Users.ToList().ForEach(u =>
            {
                pushMessageDtoList.Add(
                    new T_BD_PushMessage()
                    {
                        IsRead = pushmsg.IsRead,
                        MsgTitle = pushmsg.MSGTITLE,
                        MsgContent = pushmsg.MSGCONTENT,
                        MsgType = Convert.ToInt32(pushmsg.MSGTYPE),
                        MsgSendType = pushmsg.MsgSendType,
                        ExpirationTime = pushmsg.ExpirationTime,
                        Userid = GetUserIdListByUserAccount(u)
                    }
                    );
            });

            IList<int> pushMessageEntiyIdList = new List<int>();
            pushMessageDtoList.ToList().ForEach(u =>
            {
                //save db
                //entiesrepository.Add(u);
                //entiesrepository.Save();

              //  pushMessageEntiyIdList.Add(dbEntity.Id);
            }
            );

            return pushMessageEntiyIdList.ToArray();
 
        }

        /// <summary>
        /// Sends the push message.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private bool SendPushMessageToMQ(PushMsg msg)
        {
            var activemq = CreateActiveMQInstance();
            int sendflag = activemq.SendMessage<PushMsg>(msg);

            return sendflag > 0 ? true : false;
        }

        protected bool PushMessages(PushMsg[] pushMsgs)
        {
            if (pushMsgs == null && pushMsgs.Length > 0)
            {
                throw new ArgumentNullException("Push Message model should not be null");
            }

            bool isSendSuccess = false;
            foreach (var messagebody in pushMsgs)
            {
                //Write Db and get Pk list
                int[] results = InsertPushMessage(messagebody);
                if (results == null && results.Length == 0)
                {
                    throw new ArgumentException("Insert PushMessage Model to Db Fail");
                }

                //set Id of Db to Common MQ model 
                messagebody.Id = results.FirstOrDefault();
                //send to front
                isSendSuccess = PushMessageToMQ(messagebody);
            }
            return isSendSuccess;
        }

        protected bool PushMessageToMQ(PushMsg pushmsg)
        {
            if (pushmsg == null)
            {
                throw new ArgumentNullException("PushMsg object should not be null");
            }

            if (pushmsg.Users == null)
            {
                throw new ArgumentNullException("User list should not be null");
            }

            //判断过期logic
            if (pushmsg.ExpirationTime >= DateTime.Now)
            {
                // Clients.
                return SendPushMessageToMQ(pushmsg);
            }
            return false;
        }

        private static ActiveMQAdapter<PushMsg> CreateActiveMQInstance()
        {
            var activemq = new ActiveMQAdapter<PushMsg>(MQConfig.MQIpAddress, MQConfig.QueueDestination);
            return activemq;
        }
    }
}