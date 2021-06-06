using Progra1Bot.Clases.Alumnos;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace Progra1Bot.Clases
{
   public  class clsEjemplo2
    {
        private static TelegramBotClient Bot;

        public  async Task IniciarTelegram()
        {
            Bot = new TelegramBotClient("1818259524:AAH13A2cusko24Ku_2AtODT5_8dsFaIJM-c");
           
            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            Bot.OnMessage += BotCuandoRecibeMensajes;
            Bot.OnMessageEdited += BotCuandoRecibeMensajes;
            Bot.OnReceiveError += BotOnReceiveError;

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"escuchando solicitudes del BOT @{me.Username}");

           

            Console.ReadLine();
            Bot.StopReceiving();
        }

        // cuando recibe mensajes
        private static async void BotCuandoRecibeMensajes(object sender, MessageEventArgs messageEventArgumentos)
        {
            var ObjetoMensajeTelegram = messageEventArgumentos;
            var mensajes = ObjetoMensajeTelegram.Message;

            string mensajeEntrante = mensajes.Text;


            string respuesta = "ESTE BOT ESTA HECHO PARA CONSULTA DE VENTAS POR FAVOR INGRESA EL NUMERO DE VENTA";
            if (mensajes == null || mensajes.Type != MessageType.Text)
                return;

            Console.WriteLine($"Recibiendo Mensaje del chat {ObjetoMensajeTelegram.Message.Chat.Id}.");
            Console.WriteLine($"Dice {ObjetoMensajeTelegram.Message.Text}.");

            //tolower
            var Id_Venta = mensajes.Text;
            if (mensajes.Text.ToLower().Contains(Id_Venta))
            {
                DataTable ConsultarBasedeDatos(string condicion = "1=1")
                {
                    ConexionSql cn = new ConexionSql();
                    DataTable dt = new DataTable();
                    string sentencia = $"SELECT * FROM FARMACIA_FRONTERA WHERE {condicion}";
                    dt = cn.consultaTablaDirecta(sentencia);
                    return dt;
                }

                string Id = mensajes.Text;
                string condicion = $"ID_VENTAS = {Id}";
                DataTable dt = ConsultarBasedeDatos(condicion);

                if (dt.Rows.Count > 0)
                {
                    string producto = dt.Rows[0].Field<string>("PRODUCTO");
                    int precio = dt.Rows[0].Field<int>("PRECIO");
                    int cantidad = dt.Rows[0].Field<int>("CANTIDAD");
                    int telefono = dt.Rows[0].Field<int>("TELEFONO");
                    string cliente = dt.Rows[0].Field<string>("CLIENTE");
                    var TOTAL = precio * cantidad;
                    respuesta = $"PRODUCTO VENDIDO : {producto}\nPRECIO : {precio}\nCANTIDAD : {cantidad}\nTELEFONO : {telefono}\nCLIENTE: {cliente}\nTOTAL PAGADO: {TOTAL}";
                }
                else
                {
                    respuesta = "LO SIENTO EL NUMERO DE VENTA QUE INGRESO NO EXISTE";
                }
            }


            if (!String.IsNullOrEmpty(respuesta))//    
            {
                await Bot.SendTextMessageAsync(
                    chatId: ObjetoMensajeTelegram.Message.Chat,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    text: respuesta

            );
            }

        } // fin del metodo de recepcion de mensajes



        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("UPS!!! Recibo un error!!!: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }


    }
}
