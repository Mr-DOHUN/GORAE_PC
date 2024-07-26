using NModbus;
using NModbus.Device;
using NModbus.IO;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows;
using System;

namespace GORAE_REF_SYSTEM
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;
        private SerialPort port;
        private IModbusSerialMaster master;
        private DispatcherTimer readTimer;
        private DispatcherTimer pvTimer;
        private int dataPointIndex = 0; // 데이터 포인트 인덱스
        private bool isInitialized = false; // 초기화 상태를 추적하는 변수

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel(); // _viewModel 인스턴스 할당
            DataContext = _viewModel;

            // 현재 시간표시 타이머 설정
            pvTimer = new DispatcherTimer();
            pvTimer.Interval = new TimeSpan(0, 0, 1); // 1초
            pvTimer.Tick += PvTimer_Tick;
            pvTimer.Start();

            ModBusOpen();

            // 주기적으로 PLC 데이터를 읽기 위한 타이머 설정
            readTimer = new DispatcherTimer();
            readTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); // 0.1초
            readTimer.Tick += async (sender, e) => await ReadTimer_TickAsync();
            readTimer.Start();
        }

        private void PvTimer_Tick(object sender, EventArgs e)
        {
            dclock.Text = DateTime.Now.ToString();
        }

        private async Task ReadTimer_TickAsync()
        {
            await Task.Run(() => PlcDataRead());
        }

        public void PlcDataRead()
        {
            try
            {
                byte slaveId = 1;
                ushort startAddress = 0;
                ushort numRegisters = 21;

                ushort[] registers = master.ReadInputRegisters(slaveId, startAddress, numRegisters);

                Dispatcher.Invoke(() =>
                {
                    LstBox.Items.Clear();
                    for (int i = 0; i < registers.Length; i++)
                    {
                        LstBox.Items.Add($"PLC MODBUS {i}번째 DATA : {registers[i]}");
                    }
                    LstBox.Items.Add(" ");

                    if (registers.Length > 20 && registers[20] == 2)
                    {
                        if (registers.Length > 0) refSelValue.Text = registers[0].ToString();
                        if (registers.Length > 1)
                        {
                            double formattedValue = registers[1] / 10.0; // 소수점 한 자리 형식으로 변환
                            refInjcetValue.Text = formattedValue.ToString("F1"); // #.# 형식으로 표시
                        }
                        if (registers.Length > 2)
                        {
                            double formattedValue = registers[2] / 10.0; // 소수점 한 자리 형식으로 변환
                            refTimeValue.Text = formattedValue.ToString("F1"); // #.# 형식으로 표시
                        }
                        if (registers.Length > 4)
                        {
                            int dwordValue = ((int)registers[4] << 16) | registers[3];
                            double formattedValue = dwordValue / 10000.0; // 소수점 형식으로 변환
                            refVcValue.Text = formattedValue.ToString("F4"); // ###.#### 형식으로 표시
                        }
                        if (registers.Length > 5)
                        {
                            double formattedValue = registers[5] / 10.0; // 소수점 한 자리 형식으로 변환
                            refScaleValue.Text = formattedValue.ToString("F1"); // #.# 형식으로 표시
                        }
                    }
                    else if (registers.Length > 20 && registers[20] == 1 && !isInitialized)
                    {
                        if (registers.Length > 0) refSelValue.Text = "0";
                        if (registers.Length > 1) refInjcetValue.Text = "0";
                        if (registers.Length > 2) refTimeValue.Text = "0";
                        if (registers.Length > 3) refVcValue.Text = "0";
                        if (registers.Length > 4) refScaleValue.Text = "0";

                        isInitialized = true; // 초기화 상태를 true로 설정
                    }
                    else if (registers.Length > 20 && registers[20] == 1)
                    {
                        // 데이터 포인트 인덱스를 X축 값으로 사용
                        double xValue = dataPointIndex * 0.1; // 0.1초 단위

                        // 데이터 추가
                        _viewModel.UpdatePlotModel(_viewModel.AccumInTempData, xValue, registers[8] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.AccumInPressData, xValue, registers[9] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.AccumOutTempData, xValue, registers[10] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.AccumOutPressData, xValue, registers[11] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.BoosterInTempData, xValue, registers[12] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.BoosterInPressData, xValue, registers[13] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.BoosterOutTempData, xValue, registers[14] / 10.0);
                        _viewModel.UpdatePlotModel(_viewModel.BoosterOutPressData, xValue, registers[15] / 10.0);

                        dataPointIndex++; // 인덱스 증가

                        // 그래프 다시 그리기
                        _viewModel.RefreshPlots();
                    }
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    MessageBox.Show($"PLC 연결 이상: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                });
            }
        }

        public void ModBusOpen()
        {
            try
            {
                port = new SerialPort("COM5");
                port.BaudRate = 19200;
                port.DataBits = 8;
                port.Parity = Parity.None;
                port.StopBits = StopBits.One;

                var adapter = new SerialPortAdapter(port);
                adapter.Open();

                var factory = new ModbusFactory();
                master = factory.CreateRtuMaster(adapter);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Modbus 연결 실패: {ex.Message}", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown(); // 프로그램 종료
            }
        }

        private void Adapter_PortOpened(object sender, EventArgs e)
        {
            MessageBox.Show("PLC 연결 성공", "연결", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}