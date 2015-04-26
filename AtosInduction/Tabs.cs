using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtosInduction
{
    public class Tab
    {
        public string content { get; private set; }
        public string description { get; private set; }
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
            Add(new Tab("Home", "The central to Atos", "http://atos.net"));
            Add(new Tab("Who Are Atos?", "Learn more about the company", "http://atos.net/en-us/home/we-are.html"));
            Add(new Tab("Where do I fit?", "Where do you belong in Atos?", "http://atos.net/en-us/home/we-do/application-management.html"));
            Add(new Tab("Atos customers", "Because we care", "http://atos.net/en-us/home/we-are/our-customers.html"));
            Add(new Tab("Company structure", "Bringing together people and technology", "http://atos.net/en-us/home/we-are/company-profile.html"));
            Add(new Tab("Operational targets", "Aiming to achieve", "http://atos.net/en-us/home/we-are/company-profile/corporate-values.html"));
            Add(new Tab("Where next?", "Where do we go from here?", "http://atos.net/en-us/home/we-are/insights-innovation.html"));
        }
    }

    public class MSTabs : ObservableCollection<Tab>
    {
        public MSTabs()
        {
            Add(new Tab("Home", "About Managed Services", "http://atos.net/en-us/home/your-business/utilities/managed-services-for-utilities.html"));
            Add(new Tab("Structure", "Bringing together Managed Services", "http://atos.net/en-us/home/your-business/manufacturing.html"));
            Add(new Tab("Operational Targets", "We aim to achieve.", "http://atos.net/en-us/home/your-business/telecommunications/telecom-managed-operations.html"));
            Add(new Tab("Essentials", "The absolute neccessities", "http://atos.net/en-us/home/we-do/business-integration-solutions.html"));
            Add(new Tab("Departement Induction", "Every journey has a beginning", "http://atos.net/en-us/home/we-do/project-services.html"));
            Add(new Tab("Where Next?", "Where do we go from here?", "http://atos.net/en-us/home/olympic-games.html"));
        }
    }
}