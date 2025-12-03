using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Doctor_Appointment_System.Models
{
    class TimeSlot
    {
        private long timeSlotId; // уникальный идентификатор временного слота.
        private long doctorId; // идентификатор врача.
        private DateTime startTime; // начало временного интервала.
        private DateTime endTime; // конец временного интервала.
        private bool isAvailable; // доступен ли слот для записи.

        public TimeSlot(long slotId, long docId, DateTime start, DateTime end, bool isAvailable)
        {
            TimeSlotId = slotId;
            DoctorId = docId;
            StartTime = start;
            EndTime = end;
            IsAvailable = isAvailable;
        }

        public long TimeSlotId
        {
            get => timeSlotId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор временного слота не может быть меньше 0.");
                timeSlotId = value;
            }
        }

        public long DoctorId
        {
            get => doctorId;
            set
            {
                if (value < 0) throw new ArgumentException("Идентификатор врача не может быть меньше 0.");
                doctorId = value;
            }
        }

        public DateTime StartTime
        {
            get => startTime;
            set
            {
                // Проверка на минимальную дату
                if (value < new DateTime(2000, 1, 1))
                    throw new ArgumentException("Дата начала не может быть раньше 2000 года");

                // Проверка что время не в прошлом (если это актуально)
                if (value < DateTime.Now.AddMinutes(-5)) // допускаем небольшой запас в 5 минут
                    throw new ArgumentException("Время начала не может быть в прошлом");

                // Проверка что это рабочее время (например, с 8:00 до 20:00)
                if (value.TimeOfDay < TimeSpan.FromHours(8) || value.TimeOfDay > TimeSpan.FromHours(20))
                    throw new ArgumentException("Время начала должно быть в рабочее время (с 8:00 до 20:00)");

                startTime = value;

                // Автоматическая проверка согласованности с EndTime
                if (endTime != DateTime.MinValue && startTime >= endTime)
                {
                    throw new ArgumentException("Время начала должно быть раньше времени окончания");
                }
            }
        }

        public DateTime EndTime
        {
            get => endTime;
            set
            {
                // Проверка на минимальную дату
                if (value < new DateTime(2000, 1, 1))
                    throw new ArgumentException("Дата окончания не может быть раньше 2000 года");

                // Проверка что это рабочее время
                if (value.TimeOfDay < TimeSpan.FromHours(8) || value.TimeOfDay > TimeSpan.FromHours(20))
                    throw new ArgumentException("Время окончания должно быть в рабочее время (с 8:00 до 20:00)");

                // Проверка что продолжительность приема разумная (от 15 минут до 4 часов)
                if (startTime != DateTime.MinValue)
                {
                    TimeSpan duration = value - startTime;
                    if (duration < TimeSpan.FromMinutes(15))
                        throw new ArgumentException("Продолжительность приема не может быть меньше 15 минут");
                    if (duration > TimeSpan.FromHours(4))
                        throw new ArgumentException("Продолжительность приема не может быть больше 4 часов");
                }

                endTime = value;

                // Проверка согласованности со StartTime
                if (startTime != DateTime.MinValue && startTime >= endTime)
                {
                    throw new ArgumentException("Время окончания должно быть позже времени начала");
                }
            }
        }

        public bool IsAvailable
        {
            get => isAvailable;
            set => isAvailable = value;
        }

        // Дополнительное свойство для получения продолжительности
        public TimeSpan Duration
        {
            get
            {
                if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
                    return endTime - startTime;
                return TimeSpan.Zero;
            }
        }

        // Метод для установки обоих времен сразу с проверкой
        public void SetTimeInterval(DateTime start, DateTime end)
        {
            if (start >= end)
                throw new ArgumentException("Время начала должно быть раньше времени окончания");

            if (start.Date != end.Date)
                throw new ArgumentException("Время начала и окончания должны быть в один день");

            StartTime = start;
            EndTime = end;
        }
    }
}
