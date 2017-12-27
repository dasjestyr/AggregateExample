using System;

namespace AggregateExample
{
    public class Program
    {
        private readonly TestRepository<RootA> _testRootARepository = new TestRepository<RootA>();
        private readonly TestRepository<RootB> _testRootBRepository = new TestRepository<RootB>();
        private readonly Guid ROOTB_ID = Guid.NewGuid(); // so we can look it up later

        private static void Main(string[] args)
        {
            var p = new Program();
            p.Test1();
            p.Test2();
            Console.ReadKey();
        }

        public void Test1()
        {
            /* ****************** TEST 1 -- Testing creation and manipulation from root A *************************
             *  'Transaction 1': RootA-RootB-NonRootEntity aggregate where the root is RootA
             *  
             *  We will create RootB through RootB, save it, and then replay it to prove that RootB
             *  is being recreated properly, internally.
             ******************************************************************************************************/
             
            Console.WriteLine("--TEST 1: Creating new RootA:");
            RootA rootA1 = new RootA();
            rootA1.CreateB(ROOTB_ID);
            rootA1.TestB();
            rootA1.TestB();

            _testRootARepository.Save(rootA1);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("--Replaying first RootA (should be identical to above):");

            // the child root B is found because the events were saved to root A's stream
            // this will emit the events to the console again so you can see RootB being recreated
            RootA rootA2 = _testRootARepository.GetById(rootA1.Id);
        }

        public void Test2()
        {
            /* ****** TEST 2 -- Testing creation and manipulation from RootA, try to recall RootB directly *****
             *  We're going to test the scenario where the aggreate root changes with the service context, or rather
             *  the rule that 'Aggregate Roots can contain other aggregate roots."
             *  
             *  'Service Context 1': RootA-RootB-NonRootEntity aggregate where the root is RootA
             *  'Service Context 2': RootB-NonRootEntity aggregate where the root is RootB. This is simulating
             *  a request that happens some time after Transaction 1.
             *  
             ******************************************************************************************************/
            _testRootARepository.Clear();

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("--TEST 2: Creating new RootA:");

            // Service Context 1
            var rootA3 = new RootA();
            rootA3.CreateB(ROOTB_ID);
            rootA3.TestB();
            _testRootARepository.Save(rootA3);

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("--Looking up the RootB that was created by the previous RootA");

            // Service Context 2 (different request)
            var rootB = _testRootBRepository.GetById(ROOTB_ID);
            if (rootB == null)
            {
                // it is null because its creation events live on RootA's event stream
                Console.WriteLine("Root B was not found");
            }
            else
            {
                rootB.CreateTestEntity();
            }
        }
    }
}
