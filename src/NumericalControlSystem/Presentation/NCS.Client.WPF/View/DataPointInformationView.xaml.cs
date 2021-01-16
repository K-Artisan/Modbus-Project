using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using NCS.Infrastructure.Logging;
using NCS.Infrastructure.UnitOfWork;
using NCS.Infrastructure.Ioc;

using NCS.Service.Messaging.DataPointService;
using NCS.Service.ServiceInterface;
using NCS.Service.SeviceImplementation;
using System.Windows.Automation;

namespace NCS.Client.WPF.View
{
    /// <summary>
    /// DataPointInformationView.xaml 的交互逻辑
    /// </summary>
    public partial class DataPointInformationView : UserControl
    {
        private IDataPointService dataPointService = null;

        public DataPointInformationView()
        {
            InitializeComponent();

            this.dgDataPiontInformation.ColumnWidth = DataGridLength.SizeToCells;

            //非Ioc，要写这写代码，很是繁琐。
            //IUnitOfWork unitOfWork = new NHUnitOfWork();
            //IDataPointRepository dataPointRepository = new DataPointRepository(unitOfWork);
            //dataPointService = new DataPointService(dataPointRepository);

            this.dataPointService = IocContainerFactory.GetUnityContainer().Resolve<IDataPointService>();
        }

        private void BtGetDataPointInformation_OnClick(object sender, RoutedEventArgs e)
        {
            ShowDataPionInformation();
        }

        private void ShowDataPionInformation()
        {
            GetAllDataPointsInfoResponse getAllDataPointsInfoResponse =
                this.dataPointService.GetAllDataPointInfo();

            if (getAllDataPointsInfoResponse != null && getAllDataPointsInfoResponse.ResponseSucceed)
            {
                this.dgDataPiontInformation.ItemsSource = getAllDataPointsInfoResponse.DataPointInfoViews;

                //分组
                ICollectionView view = CollectionViewSource.GetDefaultView(this.dgDataPiontInformation.ItemsSource);
                view.GroupDescriptions.Add(new PropertyGroupDescription("ModuleId"));

                //排序
                ICollectionView sortView = CollectionViewSource.GetDefaultView(this.dgDataPiontInformation.ItemsSource);
                sortView.SortDescriptions.Add(new SortDescription("Number", ListSortDirection.Ascending));
            }
        }

        private void BtClearDataPointInformation_OnClick(object sender, RoutedEventArgs e)
        {
            this.dgDataPiontInformation.ItemsSource = null;
        }

    }
}
