using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtualSuspect.KnowledgeBase;
namespace VirtualSuspect.Query
{
    public class GetLocationFocusPredicate : IFocusPredicate{

        public Func<EventNode, QueryResult.Result> CreateFunction() {
            return delegate (EventNode node) {    

                return new QueryResult.Result(node.Location, 1 , KnowledgeBaseManager.DimentionsEnum.Location);

            };
        }

        public KnowledgeBaseManager.DimentionsEnum GetSemanticRole() {
            return KnowledgeBaseManager.DimentionsEnum.Location;
        }
    }
}
