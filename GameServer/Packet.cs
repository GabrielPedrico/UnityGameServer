using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace GameServer
{
    /// <summary>Enviado do server para o client.</summary>
    public enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        playerRotation
    }

    /// <summary>Enviado do client pro server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        playerMovement
    }

    public class Packet : IDisposable
    {
        private List<byte> buffer;
        private byte[] readableBuffer;
        private int readPos;

        /// <summary>Cria um novo pacote de dados vazio (sem um ID).</summary>
        public Packet()
        {
            buffer = new List<byte>(); // Initialize buffer
            readPos = 0; // Set readPos to 0
        }

        /// <summary>Cria um novo pacote de dados com um ID. Usado para enviar.</summary>
        /// <param name="_id">O ID do pacote de dados.</param>
        public Packet(int _id)
        {
            buffer = new List<byte>(); // Inicializa buffer
            readPos = 0; // Define readPos para 0

            Write(_id); // Escreve o ID do pacote de dados para o Buffer
        }

        /// <summary>Cria um pacote de dados aonde os dados podem ser lidos. Usado para receber.</summary>
        /// <param name="_data">Os bytes para serem adicionados ao pacote.</param>
        public Packet(byte[] _data)
        {
            buffer = new List<byte>(); // Inicializar buffer
            readPos = 0; // Define readPos para 0

            SetBytes(_data);
        }

        #region Functions
        /// <summary>Define o conteudo dos pacotes e prepara eles para leitura.</summary>
        /// <param name="_data">Os bytes a serem adicionados ao pacote.</param>
        public void SetBytes(byte[] _data)
        {
            Write(_data);
            readableBuffer = buffer.ToArray();
        }

        /// <summary>Insere o tamanho do conteudo do pacote na inicializacao do buffer.</summary>
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count)); // Insere o tamanho do pacote de dados em bytes no inicio
        }

        /// <summary>Insere a int atribuida no inicio do buffer.</summary>
        /// <param name="_value"> Int a ser inserida.</param>
        public void InsertInt(int _value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(_value)); // Insere a int no começo do buffer
        }

        /// <summary>Pega o conteudo do pacote de dados atraves de uma array.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();
            return readableBuffer;
        }

        /// <summary>Pega o tamanho do pacote de dados.</summary>
        public int Length()
        {
            return buffer.Count; // Retorna o tamanho do buffer
        }

        /// <summary>Pega o tamanho da data não lida no pacote.</summary>
        public int UnreadLength()
        {
            return Length() - readPos; // Retorna o tamanho restante (Nao lido)
        }

        /// <summary>Reseta a instancia do pacote de dados para permitir ser reutilizado.</summary>
        /// <param name="_shouldReset">Se deve ou nao redefinir o pacote.</param>
        public void Reset(bool _shouldReset = true)
        {
            if (_shouldReset)
            {
                buffer.Clear(); // Limpar os buffers
                readableBuffer = null;
                readPos = 0; // Reseta o readPos
            }
            else
            {
                readPos -= 4; // "Nao le" a ultima int lida
            }
        }
        #endregion

        #region Write Data
        /// <summary>Adiciona o byte no pacote.</summary>
        /// <param name="_value">Byte a ser adicionado.</param>
        public void Write(byte _value)
        {
            buffer.Add(_value);
        }
        /// <summary>Adiciona uma array de bytes para o pacote.</summary>
        /// <param name="_value">Array de bytes a ser adicionada.</param>
        public void Write(byte[] _value)
        {
            buffer.AddRange(_value);
        }
        /// <summary>Adiciona um atalho para o pacote.</summary>
        /// <param name="_value">O atalho a ser adicionado.</param>
        public void Write(short _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adiciona uma int ao pacote.</summary>
        /// <param name="_value">Int a ser adicionada.</param>
        public void Write(int _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adiciona uma long ao pacote.</summary>
        /// <param name="_value">Long a ser adicionada.</param>
        public void Write(long _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adiciona uma float ao pacote.</summary>
        /// <param name="_value">Float a ser adicionada.</param>
        public void Write(float _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adiciona uma bool ao pacote.</summary>
        /// <param name="_value">Bool a ser adicionada.</param>
        public void Write(bool _value)
        {
            buffer.AddRange(BitConverter.GetBytes(_value));
        }
        /// <summary>Adiciona uma string ao pacote.</summary>
        /// <param name="_value">String a ser adicionada.</param>
        public void Write(string _value)
        {
            Write(_value.Length); // Adiciona o tamanho da string ao pacote
            buffer.AddRange(Encoding.ASCII.GetBytes(_value)); // Adiciona a string em si
        }
        /// <summary>Adiciona um Vector3 ao pacote.</summary>
        /// <param name="_value">Vector3 a ser adicionado.</param>
        public void Write(Vector3 _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
        }
        /// <summary>Adiciona um Quaternion ao pacote.</summary>
        /// <param name="_value">O Quaternion a ser adicionado.</param>
        public void Write(Quaternion _value)
        {
            Write(_value.X);
            Write(_value.Y);
            Write(_value.Z);
            Write(_value.W);
        }
        #endregion

        #region Read Data
        /// <summary>Le o byte do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public byte ReadByte(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If tiver bytes nao lidos
                byte _value = readableBuffer[readPos]; // Pega a posicao do byte em readPos' position
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Aumenta readPos em 1
                }
                return _value; // Retorna o byte
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'byte'!");
            }
        }

        /// <summary>Le uma array de bytes do pacote de dados.</summary>
        /// <param name="_length">O tamanho da array de bytes.</param>
        /// <param name="_moveReadPos">Se deve ou não mover a posicao de leitura do buffer.</param>
        public byte[] ReadBytes(int _length, bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If existem bytes nao lidos
                byte[] _value = buffer.GetRange(readPos, _length).ToArray(); // Pega os bytes na posicao em readPos' com alcance ate _length
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += _length; // Aumenta o readPos em _length
                }
                return _value; // Retorna os bytes
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'byte[]'!");
            }
        }

        /// <summary>Le um pedaco dos bytes.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public short ReadShort(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                short _value = BitConverter.ToInt16(readableBuffer, readPos); // Converte os bytes para um short
                if (_moveReadPos)
                {
                    // If _moveReadPos is true e existem bytes nao lidos
                    readPos += 2; // Aumenta o readPos em 2
                }
                return _value; // Retorna o short
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'short'!");
            }
        }

        /// <summary>Le uma int do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public int ReadInt(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If existem bytes nao lidos
                int _value = BitConverter.ToInt32(readableBuffer, readPos); // Converte bytes para int
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Aumenta o readPos em 4
                }
                return _value; // Retorna a int
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'int'!");
            }
        }

        /// <summary>Le uma Long de um pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public long ReadLong(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                long _value = BitConverter.ToInt64(readableBuffer, readPos); // Converte os bytes para long
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 8; // Aumenta o readPos em 8
                }
                return _value; // Retorna a long
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'long'!");
            }
        }

        /// <summary>Le uma Float do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public float ReadFloat(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If tiver pacotes nao lidos
                float _value = BitConverter.ToSingle(readableBuffer, readPos); // Converte os bytes para float
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 4; // Aumenta o readPos em 4
                }
                return _value; // Retorna a float
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'float'!");
            }
        }

        /// <summary>Le uma Bool do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public bool ReadBool(bool _moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                // If there are unread bytes
                bool _value = BitConverter.ToBoolean(readableBuffer, readPos); // Converte os byte para bool
                if (_moveReadPos)
                {
                    // If _moveReadPos is true
                    readPos += 1; // Aumenta o readPos em 1
                }
                return _value; // Retorna a bool
            }
            else
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'bool'!");
            }
        }

        /// <summary>Le uma string do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public string ReadString(bool _moveReadPos = true)
        {
            try
            {
                int _length = ReadInt(); // Pega o tamanho da string
                string _value = Encoding.ASCII.GetString(readableBuffer, readPos, _length); // Converte bytes para string
                if (_moveReadPos && _value.Length > 0)
                {
                    // If _moveReadPos is true a string nao esta vazia
                    readPos += _length; // Aumenta o readPos pelo tamanho da string
                }
                return _value; // Retorna a string
            }
            catch
            {
                throw new Exception("Nao foi possivel ler o valor do tipo 'string'!");
            }
        }

        /// <summary>Le um Vector3 do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public Vector3 ReadVector3(bool _moveReadPos = true)
        {
            return new Vector3(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }

        /// <summary>Le um Quaternion do pacote.</summary>
        /// <param name="_moveReadPos">Se deve ou não mover a posição de leitura do buffer.</param>
        public Quaternion ReadQuaternion(bool _moveReadPos = true)
        {
            return new Quaternion(ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos), ReadFloat(_moveReadPos));
        }
        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool _disposing)
        {
            if (!disposed)
            {
                if (_disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

