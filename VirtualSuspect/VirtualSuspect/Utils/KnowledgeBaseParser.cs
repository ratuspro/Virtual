﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VirtualSuspect.Utils
{
    public static class KnowledgeBaseParser{

        public static KnowledgeBase parseFromFile(string filePath) {

            XmlDocument xmlFile = new XmlDocument();

            xmlFile.Load(filePath);

            return parseFromXml(xmlFile.DocumentElement);

        }

        public static KnowledgeBase parseFromXml(XmlNode xmlRoot) {

            KnowledgeBase kb = new KnowledgeBase();

            Dictionary<uint, EntityNode> entities = new Dictionary<uint, EntityNode>();

            //Extract all entities
            XmlNodeList entityNodesList = xmlRoot.SelectNodes("entity");

            foreach (XmlNode entityNode in entityNodesList) {

                uint id = UInt32.Parse(entityNode.SelectSingleNode("id").InnerText);

                string type = entityNode.SelectSingleNode("type").InnerText;

                if (type == "TimeSpan") {

                    string beginTime = entityNode.SelectSingleNode("begin").InnerText;

                    string endTime = entityNode.SelectSingleNode("end").InnerText;

                    EntityDto newEntityDto = new EntityDto(beginTime + ":TO:" + endTime, "TimePeriod");

                    entities.Add(id, kb.CreateNewEntity(newEntityDto)); //Source of Polymorphism problems

                }
                else if (type == "TimeInstant") {

                    string time = entityNode.SelectSingleNode("value").InnerText;

                    EntityDto newEntityDto = new EntityDto(time, "TimeInstant");

                    entities.Add(id, kb.CreateNewEntity(newEntityDto)); //Source of Polymorphism problems

                }
                else {

                    string value = entityNode.SelectSingleNode("value").InnerText;

                    EntityDto newEntityDto = new EntityDto(value, type);

                    entities.Add(id, kb.CreateNewEntity(newEntityDto));

                }

            }

            //Extract all actions from each episode
            XmlNodeList eventsNodesList = xmlRoot.SelectNodes("event");

            foreach (XmlNode eventXmlNode in eventsNodesList) {

                //Get Action if it does not exist yet
                ActionNode newActionNode = kb.GetOrCreateAction(eventXmlNode.SelectSingleNode("action").InnerText);

                //Get Time
                XmlNode timeXmlNode = eventXmlNode.SelectSingleNode("association[@relation='Time']");
                EntityNode timeNode = entities[UInt32.Parse(timeXmlNode.SelectSingleNode("entity").Attributes["id"].Value)];

                //Get Location
                XmlNode locationXmlNode = eventXmlNode.SelectSingleNode("association[@relation='Location']");
                EntityNode locationNode = entities[UInt32.Parse(locationXmlNode.SelectSingleNode("entity").Attributes["id"].Value)];

                //Make new EventNode
                EventDto eventDto = new EventDto(newActionNode, timeNode, locationNode);

                //Make associations
                XmlNodeList associationNodesList = eventXmlNode.SelectNodes("association");

                foreach (XmlNode associationNode in associationNodesList) {

                    string associationRelation = associationNode.Attributes["relation"].Value;

                    uint enID = UInt32.Parse(associationNode.SelectSingleNode("entity").Attributes["id"].Value);

                    if(associationRelation == "Agent") {
                        eventDto.AddAgent(entities[enID]);
                    }else if (associationRelation == "Theme") {
                        eventDto.AddTheme(entities[enID]);
                    }else if (associationRelation == "Manner") {
                        eventDto.AddManner(entities[enID]);
                    }else if (associationRelation == "Reason") {
                        eventDto.AddReason(entities[enID]);
                    }

                }

                EventNode newEventNode = kb.CreateNewEvent(eventDto);

                kb.AddEventToStory(newEventNode);
            }


            return kb;

        }
    }
}
