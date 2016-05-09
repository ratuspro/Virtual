﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using VirtualSuspect.Query;
using VirtualSuspect.KnowledgeBase;
using VirtualSuspect.Handler;


namespace VirtualSuspect {

    public class VirtualSuspectQuestionAnswer : IQuestionAnswerSystem {

        public enum LieStrategy { None, Hide, AdjustEntity, AdjustEvent, Improvise}

        private LieStrategy strategy;

        public LieStrategy Strategy {

            get {
                return strategy;
            }

            set {
                strategy = value;
            }

        }

        private KnowledgeBaseManager knowledgeBase;

        public KnowledgeBaseManager KnowledgeBase {

            get {
                    return knowledgeBase;
            }

        }

        private OrderedDictionary preHandlers;

        private OrderedDictionary posHandlers;

        public VirtualSuspectQuestionAnswer(KnowledgeBaseManager kb) {

            knowledgeBase = kb;

            //Setup Handlers
            preHandlers = new OrderedDictionary();
            posHandlers = new OrderedDictionary();

            //Setup Theory of Mind to Handle the query received
            preHandlers.Add(1, new ReceiverTheoryOfMindHandler(this));

            //Setup the strategy selector to handle the decision making process related with the selection of the approach to be used
            //  Create the distribution for the strategies
            Dictionary<LieStrategy, float> distribution = new Dictionary<LieStrategy, float>();
            distribution.Add(LieStrategy.None, 5);
            distribution.Add(LieStrategy.Hide, 95);
            distribution.Add(LieStrategy.AdjustEntity, 0);
            distribution.Add(LieStrategy.AdjustEvent, 0);
            distribution.Add(LieStrategy.Improvise, 0);
            preHandlers.Add(2, new StrategySelectorHandler(this, distribution));

            //Setup the handler to modify the KnowledgeBase in function of the strategy and the question
            //preHandlers.Add(3, new ModifyKnowledgeBaseHandler(this));

            //Setup Theory of Mind to Handle the results of the query
            //   We assume that the answer is known by the user
            posHandlers.Add(1, new SenderTheoryOfMindHandler(this, true));

            //Setup the handler that filters the answer before they are sent
            posHandlers.Add(2, new FilterAnswerHandler(this));

        }

        public QueryResult Query(QueryDto query) {

            //Run Pre Handler
            foreach(IPreHandler handler in preHandlers.Values) {
                query = handler.Modify(query);
            }

            //Perform Query
            QueryResult result = new QueryResult(query);

            if (query.QueryType == QueryDto.QueryTypeEnum.YesOrNo) { //Test yes or no

                result.AddBooleanResult(FilterEvents(query.QueryConditions).Count != 0);

            }else if (query.QueryType == QueryDto.QueryTypeEnum.GetInformation) { //Test get information

                List<EventNode> queryEvents = FilterEvents(query.QueryConditions);

                //Select entities from the dimension
                foreach (IFocusPredicate focus in query.QueryFocus) {

                    result.AddResults(queryEvents.Select(focus.CreateFunction()));

                }

                //Count Cardinality
                result.CountResult();

            }

            //Run Pos Handler
            foreach (IPosHandler handler in posHandlers.Values) {
                result = handler.Modify(result);
            }

            return result;

        }
    
        internal List<EventNode> FilterEvents(List<IConditionPredicate> predicates) {

            List<EventNode> queryEvents = knowledgeBase.Story;

            //Select entities from the dimension
            foreach (IConditionPredicate predicate in predicates) {

                queryEvents = queryEvents.FindAll(predicate.CreatePredicate());
            }

            return queryEvents;
        }

    }


}