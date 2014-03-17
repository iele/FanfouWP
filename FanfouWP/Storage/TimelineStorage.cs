using FanfouWP.API.Event;
using FanfouWP.API.Items;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FanfouWP.Storage
{
    public class TimelineStorage<T> where T : FanfouWP.API.Items.Item
    {
        public delegate void WriteDataSuccessHandler(object sender, EventArgs e);
        public delegate void WriteDataFailedHandler(object sender, FailedEventArgs e);
        public delegate void ReadDataSuccessHandler(object sender, UserTimelineEventArgs<T> e);
        public delegate void ReadDataFailedHandler(object sender, FailedEventArgs e);

        public event WriteDataSuccessHandler WriteDataSuccess;
        public event WriteDataFailedHandler WriteDataFailed;
        public event ReadDataSuccessHandler ReadDataSuccess;
        public event ReadDataFailedHandler ReadDataFailed;

        public async void SaveDataToIsolatedStorage(string name, object data)
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

                var dataFolder = await localFolder.CreateFolderAsync("storage", CreationCollisionOption.OpenIfExists);
                using (var writeStream = await dataFolder.OpenStreamForWriteAsync(name + ".store", CreationCollisionOption.ReplaceExisting))
                {
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(data.GetType());
                    serializer.WriteObject(writeStream, data);
                    WriteDataSuccess(name, new EventArgs());
                    return;
                }
            }
            catch (Exception e)
            {
                WriteDataFailed(data, new FailedEventArgs());
                return;
            }
        }


        public async void ReadDataFromIsolatedStorage(string name)
        {
            try
            {
                Windows.Storage.StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var dataFolder = await localFolder.CreateFolderAsync("storage", CreationCollisionOption.OpenIfExists);

                Windows.Storage.StorageFile storageFile = await dataFolder.CreateFileAsync(name + ".store", CreationCollisionOption.OpenIfExists);
                if (storageFile != null)
                {
                    Stream rs = await storageFile.OpenStreamForReadAsync();
                    byte[] buff = new byte[rs.Length];
                    await rs.ReadAsync(buff, 0, buff.Length);
                    MemoryStream stream = new MemoryStream();
                    await stream.WriteAsync(buff, 0, buff.Length);
                    ObservableCollection<T> c = new ObservableCollection<T>();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(c.GetType());
                    c = serializer.ReadObject(stream) as ObservableCollection<T>;
                    ReadDataSuccess(name, new UserTimelineEventArgs<T>(c));
                    return;
                }
                else
                {
                    ReadDataFailed(null, new FailedEventArgs());
                    return;
                }
            }
            catch (Exception e)
            {
                ReadDataFailed(null, new FailedEventArgs());
                return;
            }
        }

    }
}
