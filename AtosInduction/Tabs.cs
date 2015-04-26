using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace AtosInduction
{
    [DataContract]
    public class Tab
    {
        [DataMember]
        public string content { get; private set; }
        public string description { get; private set; }
        [DataMember]
        public string url { get; private set; }

        public Tab(string content, string description, string url)
        {
            this.content = content;
            this.description = description;
            this.url = url;
        }
    }

    public class AtosTabs : ObservableCollection<Tab>
    {
        public AtosTabs()
        {
            Add(new Tab("Home", "description", "http://atos.net"));
            Add(new Tab("Who Are Atos?", "description", "http://atos.net/en-us/home/we-are.html"));
            Add(new Tab("Where do I fit?", "description", "http://atos.net/en-us/home/we-do/application-management.html"));
            Add(new Tab("Atos customers", "description", "http://atos.net/en-us/home/we-are/our-customers.html"));
            Add(new Tab("Company structure", "description", "http://atos.net/en-us/home/we-are/company-profile.html"));
            Add(new Tab("Operational targets", "description", "http://atos.net/en-us/home/we-are/company-profile/corporate-values.html"));
            Add(new Tab("Where next?", "description", "http://atos.net/en-us/home/we-are/insights-innovation.html"));
        }
    }

    public class MSTabs : ObservableCollection<Tab>
    {
        public MSTabs()
        {
            Add(new Tab("Home", "description", "http://atos.net/en-us/home/your-business/utilities/managed-services-for-utilities.html"));
            Add(new Tab("Structure", "description", "http://atos.net/en-us/home/your-business/manufacturing.html"));
            Add(new Tab("Operational Targets", "description", "http://atos.net/en-us/home/your-business/telecommunications/telecom-managed-operations.html"));
            Add(new Tab("Essentials", "description", "http://atos.net/en-us/home/we-do/business-integration-solutions.html"));
            Add(new Tab("Departement Induction", "description", "http://atos.net/en-us/home/we-do/project-services.html"));
            Add(new Tab("Where Next?", "description", "http://atos.net/en-us/home/olympic-games.html"));
        }
    }
}