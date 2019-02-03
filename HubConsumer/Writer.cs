using Cassandra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HubConsumer
{
    class Writer
    {
        
        public void Insert(ISession session, JObject Message) {
            // Prepare once in your application lifetime
            //session.
            var ps = session.Prepare("INSERT INTO log1 (id, ts, k, v) values (?, ?, ?, ?)");
            var x = ps.Bind(Guid.NewGuid(), Message.GetValue("ts").ToString(), Message.GetValue("k").ToString(), Message.GetValue("v").ToString());
            Console.WriteLine(x.ToString());
            // Bind the unset value in a prepared statement
            session.Execute(x);
        }
        public void Other() {



            Cluster cluster = Cluster.Builder().AddContactPoint("z00060ipsmb001.westeurope.cloudapp.azure.com").Build();
            ISession session = cluster.Connect("defks");
            var statement = session.Prepare("SELECT * FROM table where a = :a and b = :b");
            // Bind by name using anonymous types 
            session.Execute(statement.Bind(new { a = "aValue", b = "bValue" }));

            session.Execute("insert into log1 (id, ts, k, v) values ('Jones', 35, 'Austin', 'bob@example.com', 'Bob')");


            // Read Bob's information back and print to the console
            Row result = session.Execute("select * from users where lastname='Jones'").First();
            Console.WriteLine("{0} {1}", result["firstname"], result["age"]);


            // Update Bob's age and then read it back and print to the console
            session.Execute("update users set age = 36 where lastname = 'Jones'");
            result = session.Execute("select * from users where lastname='Jones'").First();
            Console.WriteLine("{0} {1}", result["firstname"], result["age"]);


            // Delete Bob, then try to read all users and print them to the console
            session.Execute("delete from users where lastname = 'Jones'");


            RowSet rows = session.Execute("select * from users");
            foreach (Row row in rows)
                Console.WriteLine("{0} {1}", row["firstname"], row["age"]);
        }
    }
}
