using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace TechInterview
{
    class CurrencyProducer
    {
        public ConcurrentQueue<int> m_values = new ConcurrentQueue<int>();
        private Random m_random = new Random();
        private Thread m_thread;

        private int m_min;
        private int m_max;

        public CurrencyProducer(int min, int max)
        {
            m_min = min;
            m_max = max;
        }

        public bool Start()
        {
            if (m_thread != null)
            {
                return false;
            }

            m_thread = new Thread(this.Generate);
            m_thread.Start();

            return true;
        }

        private void Generate()
        {
            while (true)
            {
                int value = m_random.Next(m_min, m_max);
                m_values.Enqueue(value);

                // TODO - set an event to notify that there are values available

                Thread.Sleep(5000);
            }
        }

        public int Get()
        {
            int value;
            if (!m_values.TryDequeue(out value))
            {
                return -1;
            }

            return value;
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            // Producer
            CurrencyProducer producer = new CurrencyProducer(1, 100);
            producer.Start();

            List<int> last_min_values = new List<int>();
            DateTime start_time = DateTime.Now;

            // Consumer
            while (true)
            {
                // Read all values produced until now
                int val;
                while ((val = producer.Get()) != -1)
                {
                    Console.WriteLine("Value: {0}", val);
                    last_min_values.Add(val);
                }

                // Check if at least 1 min passed
                if (start_time.AddMinutes(1) < DateTime.Now)
                {
                    Console.WriteLine("----------");
                    Console.WriteLine("Average: {0}", CalculateAverage(last_min_values));
                    Console.WriteLine("----------");

                    // Reset last minute value
                    last_min_values.Clear();
                    start_time = DateTime.Now;
                }

                Thread.Sleep(15000);
            }
        }

        static int CalculateAverage(List<int> values)
        {
            if (values.Count == 0)
            {
                return 0;
            }

            return values.Sum() / values.Count;
        }
    }
}
