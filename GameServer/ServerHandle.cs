using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    class ServerHandle // Classe que administra tudo que o servidor vai receber e como vai proceder com a instrução que chegou.
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet) // Funcao que mostra no console os players que se conectaram ID/IP/Nickname
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} conectou-se com sucesso e corresponde ao jogador: {_fromClient}, Nickname:{_username}");
            Console.ForegroundColor = ConsoleColor.White;
            if (_fromClient != _clientIdCheck)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Jogador \"{_username}\" (ID: {_fromClient}) assumiu o cliente ID errado ({_clientIdCheck})!");
                Console.ForegroundColor = ConsoleColor.White;
            }
            Server.clients[_fromClient].SendIntoGame(_username);
        }

        public static void PlayerMovement(int _fromClient, Packet _packet) // Função que administra o movimento dos players vindo do clientside
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();

            Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        }
    }
}
