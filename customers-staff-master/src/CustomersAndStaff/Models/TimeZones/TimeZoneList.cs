using System;

namespace Market.CustomersAndStaff.Models.TimeZones
{
    public static class TimeZoneList
    {
        public static readonly TimeZone[] TimeZones = {
                new TimeZone
                    {
                        Id = new Guid("e153e9e6-5c33-45b0-99b6-5e70b369ae3f"),
                        Name = "Калининград",
                        Offset = new TimeSpan(2, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("0bba1757-fd16-45f1-86e2-96f3accbd05d"),
                        Name = "Москва, Санкт-Петербург",
                        Offset = new TimeSpan(3, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("5a8ee435-4a46-49f7-aecb-b21495bdca07"),
                        Name = "Самара, Ижевск",
                        Offset = new TimeSpan(4, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("ae645052-feee-4a2c-9677-74bf27ad024e"),
                        Name = "Екатеринбург",
                        Offset = new TimeSpan(5, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("80f69678-a760-47ac-8171-c192a0f51d78"),
                        Name = "Новосибирск, Омск",
                        Offset = new TimeSpan(6, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("49c69222-337e-49ec-90ee-c90709009fe3"),
                        Name = "Красноярск",
                        Offset = new TimeSpan(7, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("b73aab92-b68b-47b9-b494-81342de6638b"),
                        Name = "Иркутск",
                        Offset = new TimeSpan(8, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("b9ec1252-8888-4acc-a5d4-cb969acee175"),
                        Name = "Якутск",
                        Offset = new TimeSpan(9, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("df226ce4-8dd0-4144-9fe1-7d5c8ee59b67"),
                        Name = "Владивосток",
                        Offset = new TimeSpan(10, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("a6e60c9f-1488-4e48-90a9-00332a6e4847"),
                        Name = "Магадан",
                        Offset = new TimeSpan(11, 0, 0),
                    },
                new TimeZone
                    {
                        Id = new Guid("d5730772-c768-4675-8182-5adf32ed05e3"),
                        Name = "Камчатка",
                        Offset = new TimeSpan(12, 0, 0),
                    },
            };
    }
}