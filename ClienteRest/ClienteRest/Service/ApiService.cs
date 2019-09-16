﻿using Acr.UserDialogs;
using ClienteRest.Model;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClienteRest.Service
{
    public class ApiService
    {
        private String url = "http://cdsapirest.somee.com/api/";
        public HttpClient cliente = new HttpClient();
        public async Task<Response> GetAll<T>(String Controller)
        {
            var wifi = Plugin.Connectivity.Abstractions.ConnectionType.WiFi;
            var connectionTypes = CrossConnectivity.Current.ConnectionTypes;
            if (!connectionTypes.Contains(wifi))
            {
                return new Response
                {
                    isSuccess = false,
                    Message= "No posee conexion a internet"
                };
            }
            try
            {
                Loading();
                var response = await cliente.GetAsync(url+Controller);
                if (!response.IsSuccessStatusCode)
                {
                    return new Response
                    {
                        isSuccess = false,
                        Message = "Error de respuesta del servidor"
                    };
                }
                var result = await response.Content.ReadAsStringAsync();
                var list = JsonConvert.DeserializeObject<ObservableCollection<T>>(result);
                return new Response
                {
                    isSuccess = true,
                    Result = list
                };
            }
            catch (Exception e)
            {
                return new Response
                {
                    isSuccess = false,
                    Message = "Error al cargar los datos"
                };
            }
        }
        public async Task<bool> Post<T>(String Controller, T item)
        {
            try
            {
                Loading();
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json,Encoding.UTF8,"application/json");
                HttpResponseMessage response = await cliente.PostAsync(url+Controller, content);
                //String mensaje = JsonConvert.DeserializeObject<String>(await response.Content.ReadAsStringAsync());
                //Debug.Print(mensaje);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> Delete<T>(String controller, int id)
        {
            try
            {
                Loading();
                HttpResponseMessage response = await cliente.DeleteAsync(url + controller+id.ToString());
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> Put<T>(String controller,int id, T item)
        {
            try
            {
                Loading();
                var json = JsonConvert.SerializeObject(item);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await cliente.PutAsync(url + controller + id, content);
                if (!response.IsSuccessStatusCode)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private async void Loading()
        {
            using (UserDialogs.Instance.Loading("Cargando", null, null, true, MaskType.Black))
            {
                await Task.Delay(4000);
            }
        }
    }
}
