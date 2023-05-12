using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirportSimulation
{
    static class Modelling
    {
        //шаг моделирования
        public static double DHour { get; set; } = 0.01;

        //кол-во часов моделирования
        public static double HourModelling { get; set; }

        //кол-во полос на посадку
        public static int NumberRunways4Landing { get; set; }
        //кол-во полос на взлет
        public static int NumberRunways4Takeoff { get; set; }

        //мат. ожидание времени появления самолета на посадку
        public static double ExpectationAppearanceLanding { get; set; }
        //среднеквадр. отклонение времени появления самолета на посадку
        public static double DeviationAppearanceLanding { get; set; }

        //мат. ожидание продолжительности посадки
        public static double ExpectationLanding { get; set; }
        //среднеквадр. отклонение продолжительности посадки
        public static double DeviationLanding { get; set; }

        //мат. ожидание времени появления самолета на взлет
        public static double ExpectationAppearanceTakeoff { get; set; }
        //среднеквадр. отклонение времени появления самолета на взлет
        public static double DeviationAppearanceTakeoff { get; set; }

        //мат. ожидание продолжительности взлета
        public static double ExpectationTakeoff { get; set; }
        //среднеквадр. отклонение продолжительности взлета
        public static double DeviationTakeoff { get; set; }


        //список кортежей (время моделирования, показатель эффективности: загруженость полос в %)
        public static List<(double, double)> LoadOfRunways;
        // список кортежей (время моделирования, показатель эффективности: среднее время ожидания посадки)
        public static List<(double, double)> AverageWaitngLanding;
        //список кортежей (время моделирования, показатель эффективности: среднее время ожидания взлета)
        public static List<(double, double)> AverageWaitngTakeoff;

        //кол-во посадок
        public static double countLanding;
        //кол-во вылетов
        public static double countTakeoff;

        //сумма времени ожидания посадок
        private static double sumWaitngLanding;

        //сумма времени ожидания вылетов
        private static double sumWaitngTakeoff;


        //очередь кортежей (самолетов на взлет/посадку, время постановки в очередь)
        private static List<(Plane, double)> queueLanding, queueTakeoff;
        //время (в часах) генерации следующего самолета на посадку/преземление
        private static double hourAppearanceNextLanding, hourAppearanceNextTakeoff;
        //время окончания (в часах) следующего взлета/приземеления для каждой полосы
        private static double[] hoursEndLanding, hoursEndTakeoff;

        //количество сгенерированных самолетов
        private static int countPlane, countJetPlane;

        private static Random rnd = new Random();

        //модуль генерации нормально распределенных чисел по центральной предельной теореме
        private static double GenerateNorm(double m, double sigma)
        {
            double sum = 0;
            for (var i = 0; i < 12; i++)
                sum += rnd.NextDouble();
            double result = m + sigma * (sum - 6);
            if (result < 0)
                return 0;
            return result;
        }

        //генерация самолета
        private static (Plane, double) GeneratePlane(double time)
        {
            if (rnd.Next(0, 2) == 0)
            {
                return (new Plane(rnd.Next(25, 100), "Basic plane #" + (countPlane++).ToString()), time);
            }
            return (new JetPlane(rnd.Next(25, 100), "Jet plane #" + (countJetPlane++).ToString()), time);
        }

        //
        private static int GetFreeLanes(double[] hours, double currentHour)
        {
            for (int i = 0; i < hours.Length; i++)
            {
                if (hours[i] <= currentHour)
                    return i;
            }
            return -1;
        }

        //кол-во свободных полос
        private static int GetCountFreeLanes(double[] hours, double currentHour)
        {
            int count = 0;
            for (int i = 0; i < hours.Length; i++)
            {
                if (hours[i] <= currentHour)
                    count++;
            }
            return count;
        }        

        //выбор следующего самолета на посадку/взлет из очереди (наименьшее кол-во топлива -> след.)
        private static (Plane, double) GetNextPlaneTuple(List<(Plane, double)> queue)
        {
            (Plane, double) nextPlane = queue[0];
            int minFuel = queue[0].Item1.Fuel;
            foreach (var plane in queue)
            {
                if (plane.Item1.Fuel < minFuel)
                {
                    minFuel = plane.Item1.Fuel;
                    nextPlane = plane;
                }
            }
            return nextPlane;
        }

        //начать моделирование
        public static void Start()
        {
            //обнуляем все поля, необходимые для моделирования системы (кроме введенных пользователем значений)
            sumWaitngLanding = sumWaitngTakeoff = countLanding = countTakeoff = 0;
            queueLanding = new List<(Plane, double)>();
            queueTakeoff = new List<(Plane, double)>();
            hourAppearanceNextLanding = GenerateNorm(ExpectationAppearanceLanding, DeviationAppearanceTakeoff);
            hoursEndLanding = new double[NumberRunways4Landing];
            hourAppearanceNextTakeoff = GenerateNorm(ExpectationAppearanceTakeoff, DeviationAppearanceTakeoff);
            hoursEndTakeoff = new double[NumberRunways4Takeoff];
            LoadOfRunways = new List<(double, double)>();
            AverageWaitngLanding = new List<(double, double)>();
            AverageWaitngTakeoff = new List<(double, double)>();

            countPlane = 0;
            countJetPlane = 0;

            double currentHour = 0;
            //пока время моделирования не закончилось
            while (currentHour <= HourModelling)
            {
                //расчитываем показатели эффективности
                int freeLanes = GetCountFreeLanes(hoursEndLanding, currentHour) + GetCountFreeLanes(hoursEndTakeoff, currentHour);
                (double, double) value1 = (currentHour, ((double)(NumberRunways4Landing + NumberRunways4Takeoff - freeLanes) / (NumberRunways4Landing + NumberRunways4Takeoff) * 100));
                LoadOfRunways.Add(value1);
                (double, double) value2 = (currentHour, sumWaitngLanding / countLanding);                
                (double, double) value3 = (currentHour, sumWaitngTakeoff / countTakeoff);
                AverageWaitngLanding.Add(value2);
                AverageWaitngTakeoff.Add(value3);

                currentHour += DHour;

                //генерируем самолеты на посадку 
                if (currentHour >= hourAppearanceNextLanding)
                {
                    queueLanding.Add(GeneratePlane(currentHour));
                    hourAppearanceNextLanding = currentHour + GenerateNorm(ExpectationAppearanceLanding, DeviationAppearanceTakeoff);
                }

                //генерируем самолеты на взлет
                if (currentHour >= hourAppearanceNextTakeoff)
                {
                    queueTakeoff.Add(GeneratePlane(currentHour));
                    hourAppearanceNextTakeoff = currentHour + GenerateNorm(ExpectationAppearanceTakeoff, DeviationAppearanceTakeoff);
                }

                int numberLanes;
                //пока имеется свободная полоса назначаем на нее самолет на посадку
                while (queueLanding.Count > 0)
                {
                    if ((numberLanes = GetFreeLanes(hoursEndLanding, currentHour)) >= 0)
                    {
                        hoursEndLanding[numberLanes] = currentHour + GenerateNorm(ExpectationLanding, DeviationLanding);
                        var removePlaneTuple = GetNextPlaneTuple(queueLanding);
                        sumWaitngLanding += currentHour - removePlaneTuple.Item2;
                        countLanding++;
                        queueLanding.Remove(removePlaneTuple);
                    }
                    else if ((numberLanes = GetFreeLanes(hoursEndTakeoff, currentHour)) >= 0)
                    {
                        hoursEndTakeoff[numberLanes] = currentHour + GenerateNorm(ExpectationLanding, DeviationLanding);
                        var removePlaneTuple = GetNextPlaneTuple(queueLanding);
                        sumWaitngTakeoff += currentHour - removePlaneTuple.Item2;
                        countLanding++;
                        queueLanding.Remove(removePlaneTuple);
                    }
                    else
                        break;
                }

                //пока имеется свободная полоса назначаем на нее самолет на посадку
                while (queueTakeoff.Count > 0)
                {
                    if ((numberLanes = GetFreeLanes(hoursEndTakeoff, currentHour)) >= 0)
                    {
                        hoursEndTakeoff[numberLanes] = currentHour + GenerateNorm(ExpectationTakeoff, DeviationTakeoff);
                        var removePlaneTuple = GetNextPlaneTuple(queueTakeoff);
                        sumWaitngTakeoff += currentHour - removePlaneTuple.Item2;
                        countTakeoff++;
                        queueTakeoff.Remove(removePlaneTuple);
                    }
                    else if ((numberLanes = GetFreeLanes(hoursEndLanding, currentHour)) >= 0)
                    {
                        hoursEndLanding[numberLanes] = currentHour + GenerateNorm(ExpectationTakeoff, DeviationTakeoff);
                        var removePlaneTuple = GetNextPlaneTuple(queueTakeoff);
                        sumWaitngTakeoff += currentHour - removePlaneTuple.Item2;
                        countTakeoff++;
                        queueTakeoff.Remove(removePlaneTuple);
                    }
                    else
                        break;
                }
            }
        }
    }
}