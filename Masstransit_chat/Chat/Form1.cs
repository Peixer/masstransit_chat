using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chat.Core;
using MassTransit;

namespace Chat
{
    public partial class Form1 : Form
    {
        private IBusControl busControl;
        readonly Guid guid = Guid.NewGuid();

        public Form1()
        {
            InitializeComponent();
            IniciarBusControl();

            id_label.Text = guid.ToString();
        }

        private void IniciarBusControl()
        {
            busControl = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });

                sbc.AutoDelete = true;
                sbc.ReceiveEndpoint(host, $"{guid.ToString()}", e =>
                {
                    e.Handler<Mensagem>(context =>
                    {
                        richTextBox.Invoke((MethodInvoker)delegate
                        {
                            richTextBox.AppendText($"{context.Message.Guid}:{context.Message.Mensagem_Enviada}\n");
                        });
                        return Task.FromResult(0);
                    });

                    e.Handler<MensagemEntrada>(context =>
                    {
                        richTextBox.Invoke((MethodInvoker)delegate
                        {
                            richTextBox.AppendText(context.Message.Mensagem + "\n");
                        });
                        return Task.FromResult(0);
                    });

                    e.Handler<MensagemEntrada>(context =>
                    {
                        richTextBox.Invoke((MethodInvoker)delegate
                        {
                            richTextBox.AppendText("Escrevendo");
                        });
                        return Task.FromResult(0);
                    });
                });
            });
            busControl.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var sendEndpoint = busControl.GetSendEndpoint(new Uri($"rabbitmq://localhost:5672/{guid}")).Result;

            sendEndpoint.Send(new Mensagem(guid, input_textBox.Text));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            busControl.Publish(new MensagemEntrada($"GUID {guid} entrou na sala"));
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            busControl.Stop();
        }

        private void InputTextBoxOnGotFocus(object sender, EventArgs eventArgs)
        {
            busControl.Publish(new MensagemEscrevendo());
        }
    }
}
