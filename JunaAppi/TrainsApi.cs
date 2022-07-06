﻿using System;
using System.Collections.Generic;
using System.Text;
using APIHelpers;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using JunaAppiReitit;
using JunaAppiLatest;

namespace JunaAppi
{
    public static class TrainsApi
    {
        const string url = "https://rata.digitraffic.fi/api/v1/";

        //tehty ryhmäkoodauksena, Johanna kirjoitusoperaattorina muu ryhmä neuvoo
        public static async Task<Juna> GetJuna(string input)
        {
            string urlParams = "trains/" + input;
            Juna response = await ApiHelper.RunAsync<Juna>(url, urlParams);
            return response;
              
        }
        //johanna teki edellisen mallin mukaan
        public static async Task<TrainTrackingLatest> GetLocation(string junanNumero)
        {

            string urlParams = $"train-tracking/latest/{junanNumero}"; //muutettu train-trackinginsta train-tracking/latest/ koska tämän perään voi sijoittaa {train_number} arvon
            TrainTrackingLatest response = await ApiHelper.RunAsync<TrainTrackingLatest>(url, urlParams);
            return response;
        }

        //Otetaan yhetyttä Apiin (Annan versio ryhmän avulla).

        public static async Task<Vaunu> HaeJunanPalvelut(string date, int junanro)
        {
            string urlParams = "compositions/" + date + "/" + junanro;
            Vaunu response = await ApiHelper.RunAsync<Vaunu>(url, urlParams);
            return response;
        }


        //Akin junan haku apista
        public static async Task<ReittiLatest[]> HaeReitti(string lahto, string saapuminen)
        {
            string urlParams = "live-trains/station/" + lahto + "/" + saapuminen;

            ReittiLatest[] reitti = await ApiHelper.RunAsync<ReittiLatest[]>(url,urlParams);

            return reitti;
        }

        //Materiaalista häpeilemättä kopioitu GetStations-metodi
        public static async Task<Station[]> GetStations()
        {
            string urlParams = "metadata/stations";

            Station[] response = await ApiHelper.RunAsync<Station[]>(url, urlParams);
            return response;
        }

        //Mari-Annen tekemä muokkaus LatestTrain-olion hakuun
        public static async Task<LatestTrain> GetTrainByNumber(string input)
        {
            string urlParams = "trains/latest/" + input;
            LatestTrain response = await ApiHelper.RunAsync<LatestTrain>(url, urlParams);
            return response;
        }

        //Mari-Annen metodi aseman hakuun asemakoodin perusteella
        public static async Task<Station> GetStationByCodeAsync(string stationShortCode)
        {
            Station[] asemat = await GetStations();
            Station response = asemat.FirstOrDefault(x => x.stationShortCode.Equals(stationShortCode, StringComparison.OrdinalIgnoreCase));

            //jos asema ei ole null, palautetaan Station-olio ja jos on, palautetaan oletusarvo
            return (response != null ? response : default);
        }

        //metodi aseman hakuun nimen perusteella /Mari-Anne
        public static async Task<Station> GetStationByNameAsync(string stationName)
        {
            Station[] asemat = await GetStations();
            Station response;
            while (true)
            {
                try
                {
                    response = asemat.FirstOrDefault(x =>
                        x.stationName.Equals(stationName, StringComparison.OrdinalIgnoreCase));
                    return response;
                }
                catch (NullReferenceException e)
                {
                    stationName += " asema";
                    response = asemat.FirstOrDefault(x =>
                        x.stationName.Equals(stationName, StringComparison.OrdinalIgnoreCase));
                    return response;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Asemaa ei löytynyt");
                    return default;
                }
            }
        }
    }
}
