using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace devide
{
    public partial class Form1 : Form
    {
        HTuple handle, Width1, Height1;
        HObject ho_Image;
        bool input_btn = false;
        HTuple hv_DLModelHandle = new HTuple(), hv_DLPreprocessParam = new HTuple();
        HTuple hv_ClassNames = new HTuple(), hv_ClassIDs = new HTuple();
        HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
        public Form1()
        {
            InitializeComponent();
            handle = hWindowControl1.HalconWindow;
            HOperatorSet.ReadImage(out ho_Image, "目标定位背景");
            HOperatorSet.GetImageSize(ho_Image, out Width1, out Height1);
            HOperatorSet.SetPart(handle, 0, 0, Height1 - 1, Width1 - 1);
            HOperatorSet.DispObj(ho_Image, handle);
        }
        private void inputbtn_Click(object sender, EventArgs e)
        {
            hv_DLModelHandle.Dispose();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "文本文件|*.hdl|所有文件|*.*"; // 设置文件过滤器，只允许选择文本文件
            openFileDialog.Multiselect = false; // 只允许选择一个文件
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName; // 获取选择的文件路径
                HOperatorSet.ReadDlModel(filePath, out hv_DLModelHandle);
                MessageBox.Show("模型已导入！");
                input_btn = true;
            }
            else
            {
                MessageBox.Show("导入模型失败！");
            }
        }
        private void devidebtn_Click(object sender, EventArgs e)
        {
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_DLPreprocessParam.Dispose(); hv_ClassNames.Dispose(); hv_ClassIDs.Dispose();
            init(hv_DLModelHandle, out hv_DLPreprocessParam, out hv_ClassNames, out hv_ClassIDs);
            //循环读图和推理
            ho_Image.Dispose();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "图片文件|*.bmp; *.pcx; *.png; *.jpg; *.gif;*.tif; *.ico; *.dxf; *.cgm; *.cdr; *.wmf; *.eps; *.emf";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                HOperatorSet.ReadImage(out ho_Image, filePath);
                //拿到实际图片大小
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                HOperatorSet.SetPart(handle, 0, 0, hv_Height - 1, hv_Width - 1);
                HDevWindowStack.Push(handle);
                HOperatorSet.DispObj(ho_Image, handle);
                devide(ho_Image, hv_DLPreprocessParam, hv_DLModelHandle, hv_ClassIDs, hv_ClassNames);
                //拿到实际图片大小
            }

            else
            {
                MessageBox.Show("请重新导入模型");
            }
    //拿到实际图片大小
    
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ho_Image.Dispose();
            hv_DLModelHandle.Dispose();
            hv_DLPreprocessParam.Dispose();
            hv_ClassNames.Dispose();
            hv_ClassIDs.Dispose();
        }

            private void calculate_dl_image_zoom_factors(HTuple hv_ImageWidth, HTuple hv_ImageHeight,
      HTuple hv_TargetWidth, HTuple hv_TargetHeight, HTuple hv_DLPreprocessParam,
      out HTuple hv_ZoomFactorWidth, out HTuple hv_ZoomFactorHeight)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ScaleWidthUnit = new HTuple(), hv_ScaleHeightUnit = new HTuple();
            HTuple hv_PreserveAspectRatio = new HTuple(), hv_Scale = new HTuple();
            HTuple hv___Tmp_Ctrl_Dict_Init_0 = new HTuple();
            // Initialize local and output iconic variables 
            hv_ZoomFactorWidth = new HTuple();
            hv_ZoomFactorHeight = new HTuple();
            try
            {
                //Calculate the unit zoom factors, which zoom the input image to 1px.
                hv_ScaleWidthUnit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleWidthUnit = 1.0 / (hv_ImageWidth.TupleReal()
                        );
                }
                hv_ScaleHeightUnit.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleHeightUnit = 1.0 / (hv_ImageHeight.TupleReal()
                        );
                }
                //
                //Calculate the required zoom factors for the available target size.
                hv_ZoomFactorWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ZoomFactorWidth = hv_TargetWidth * hv_ScaleWidthUnit;
                }
                hv_ZoomFactorHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ZoomFactorHeight = hv_TargetHeight * hv_ScaleHeightUnit;
                }
                //
                //Aspect-ratio preserving zoom is supported for model type 'ocr_detection' only.
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_0);
                HOperatorSet.SetDictTuple(hv___Tmp_Ctrl_Dict_Init_0, "comp", "ocr_detection");
                hv_PreserveAspectRatio.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PreserveAspectRatio = ((hv_DLPreprocessParam.TupleConcat(
                        hv___Tmp_Ctrl_Dict_Init_0))).TupleTestEqualDictItem("model_type", "comp");
                }
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv___Tmp_Ctrl_Dict_Init_0 = HTuple.TupleConstant(
                        "HNULL");
                }
                //
                if ((int)(hv_PreserveAspectRatio) != 0)
                {
                    //
                    //Use smaller scaling factor, which results in unfilled domain
                    //on the respective other axis.
                    hv_Scale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Scale = hv_ZoomFactorWidth.TupleMin2(
                            hv_ZoomFactorHeight);
                    }
                    //Ensure that the zoom factors result in lengths of at least 1px.
                    hv_ZoomFactorWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomFactorWidth = hv_Scale.TupleMax2(
                            hv_ScaleWidthUnit);
                    }
                    hv_ZoomFactorHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomFactorHeight = hv_Scale.TupleMax2(
                            hv_ScaleHeightUnit);
                    }
                }

                hv_ScaleWidthUnit.Dispose();
                hv_ScaleHeightUnit.Dispose();
                hv_PreserveAspectRatio.Dispose();
                hv_Scale.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_ScaleWidthUnit.Dispose();
                hv_ScaleHeightUnit.Dispose();
                hv_PreserveAspectRatio.Dispose();
                hv_Scale.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Check the content of the parameter dictionary DLPreprocessParam. 
        private void check_dl_preprocess_param(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CheckParams = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_SupportedModelTypes = new HTuple(), hv_Index = new HTuple();
            HTuple hv_ParamNamesGeneral = new HTuple(), hv_ParamNamesSegmentation = new HTuple();
            HTuple hv_ParamNamesDetectionOptional = new HTuple(), hv_ParamNamesPreprocessingOptional = new HTuple();
            HTuple hv_ParamNames3DGrippingPointsOptional = new HTuple();
            HTuple hv_ParamNamesAll = new HTuple(), hv_ParamNames = new HTuple();
            HTuple hv_KeysExists = new HTuple(), hv_I = new HTuple();
            HTuple hv_Exists = new HTuple(), hv_InputKeys = new HTuple();
            HTuple hv_Key = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_ValidValues = new HTuple();
            HTuple hv_ValidTypes = new HTuple(), hv_V = new HTuple();
            HTuple hv_T = new HTuple(), hv_IsInt = new HTuple(), hv_ValidTypesListing = new HTuple();
            HTuple hv_ValidValueListing = new HTuple(), hv_EmptyStrings = new HTuple();
            HTuple hv_ImageRangeMinExists = new HTuple(), hv_ImageRangeMaxExists = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IndexParam = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassIDsBackground = new HTuple(), hv_Intersection = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_KnownClasses = new HTuple();
            HTuple hv_IgnoreClassID = new HTuple(), hv_OptionalKeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IsInstanceSegmentation = new HTuple();
            HTuple hv_IgnoreDirection = new HTuple(), hv_ClassIDsNoOrientation = new HTuple();
            HTuple hv_SemTypes = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure checks a dictionary with parameters for DL preprocessing.
                //
                hv_CheckParams.Dispose();
                hv_CheckParams = 1;
                //If check_params is set to false, do not check anything.
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "check_params",
                    out hv_KeyExists);
                if ((int)(hv_KeyExists) != 0)
                {
                    hv_CheckParams.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "check_params", out hv_CheckParams);
                    if ((int)(hv_CheckParams.TupleNot()) != 0)
                    {

                        hv_CheckParams.Dispose();
                        hv_KeyExists.Dispose();
                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNames3DGrippingPointsOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IsInstanceSegmentation.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                }
                //
                try
                {
                    hv_DLModelType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    throw new HalconException(new HTuple(new HTuple("DLPreprocessParam needs the parameter: '") + "model_type") + "'");
                }
                //
                //Check for correct model type.
                hv_SupportedModelTypes.Dispose();
                hv_SupportedModelTypes = new HTuple();
                hv_SupportedModelTypes[0] = "3d_gripping_point_detection";
                hv_SupportedModelTypes[1] = "anomaly_detection";
                hv_SupportedModelTypes[2] = "classification";
                hv_SupportedModelTypes[3] = "detection";
                hv_SupportedModelTypes[4] = "gc_anomaly_detection";
                hv_SupportedModelTypes[5] = "ocr_recognition";
                hv_SupportedModelTypes[6] = "ocr_detection";
                hv_SupportedModelTypes[7] = "segmentation";
                hv_Index.Dispose();
                HOperatorSet.TupleFind(hv_SupportedModelTypes, hv_DLModelType, out hv_Index);
                if ((int)((new HTuple(hv_Index.TupleEqual(-1))).TupleOr(new HTuple(hv_Index.TupleEqual(
                    new HTuple())))) != 0)
                {
                    throw new HalconException(new HTuple("Only models of type '3d_gripping_point_detection', 'anomaly_detection', 'classification', 'detection', 'gc_anomaly_detection', 'ocr_recognition', 'ocr_detection' or 'segmentation' are supported"));

                    hv_CheckParams.Dispose();
                    hv_KeyExists.Dispose();
                    hv_DLModelType.Dispose();
                    hv_Exception.Dispose();
                    hv_SupportedModelTypes.Dispose();
                    hv_Index.Dispose();
                    hv_ParamNamesGeneral.Dispose();
                    hv_ParamNamesSegmentation.Dispose();
                    hv_ParamNamesDetectionOptional.Dispose();
                    hv_ParamNamesPreprocessingOptional.Dispose();
                    hv_ParamNames3DGrippingPointsOptional.Dispose();
                    hv_ParamNamesAll.Dispose();
                    hv_ParamNames.Dispose();
                    hv_KeysExists.Dispose();
                    hv_I.Dispose();
                    hv_Exists.Dispose();
                    hv_InputKeys.Dispose();
                    hv_Key.Dispose();
                    hv_Value.Dispose();
                    hv_Indices.Dispose();
                    hv_ValidValues.Dispose();
                    hv_ValidTypes.Dispose();
                    hv_V.Dispose();
                    hv_T.Dispose();
                    hv_IsInt.Dispose();
                    hv_ValidTypesListing.Dispose();
                    hv_ValidValueListing.Dispose();
                    hv_EmptyStrings.Dispose();
                    hv_ImageRangeMinExists.Dispose();
                    hv_ImageRangeMaxExists.Dispose();
                    hv_ImageRangeMin.Dispose();
                    hv_ImageRangeMax.Dispose();
                    hv_IndexParam.Dispose();
                    hv_SetBackgroundID.Dispose();
                    hv_ClassIDsBackground.Dispose();
                    hv_Intersection.Dispose();
                    hv_IgnoreClassIDs.Dispose();
                    hv_KnownClasses.Dispose();
                    hv_IgnoreClassID.Dispose();
                    hv_OptionalKeysExist.Dispose();
                    hv_InstanceType.Dispose();
                    hv_IsInstanceSegmentation.Dispose();
                    hv_IgnoreDirection.Dispose();
                    hv_ClassIDsNoOrientation.Dispose();
                    hv_SemTypes.Dispose();

                    return;
                }
                //
                //Parameter names that are required.
                //General parameters.
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesGeneral = new HTuple();
                hv_ParamNamesGeneral[0] = "model_type";
                hv_ParamNamesGeneral[1] = "image_width";
                hv_ParamNamesGeneral[2] = "image_height";
                hv_ParamNamesGeneral[3] = "image_num_channels";
                hv_ParamNamesGeneral[4] = "image_range_min";
                hv_ParamNamesGeneral[5] = "image_range_max";
                hv_ParamNamesGeneral[6] = "normalization_type";
                hv_ParamNamesGeneral[7] = "domain_handling";
                //Segmentation specific parameters.
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesSegmentation = new HTuple();
                hv_ParamNamesSegmentation[0] = "ignore_class_ids";
                hv_ParamNamesSegmentation[1] = "set_background_id";
                hv_ParamNamesSegmentation[2] = "class_ids_background";
                //Detection specific parameters.
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesDetectionOptional = new HTuple();
                hv_ParamNamesDetectionOptional[0] = "instance_type";
                hv_ParamNamesDetectionOptional[1] = "ignore_direction";
                hv_ParamNamesDetectionOptional[2] = "class_ids_no_orientation";
                hv_ParamNamesDetectionOptional[3] = "instance_segmentation";
                //Optional preprocessing parameters.
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesPreprocessingOptional = new HTuple();
                hv_ParamNamesPreprocessingOptional[0] = "mean_values_normalization";
                hv_ParamNamesPreprocessingOptional[1] = "deviation_values_normalization";
                hv_ParamNamesPreprocessingOptional[2] = "check_params";
                hv_ParamNamesPreprocessingOptional[3] = "augmentation";
                //3D Gripping Point Detection specific parameters.
                hv_ParamNames3DGrippingPointsOptional.Dispose();
                hv_ParamNames3DGrippingPointsOptional = new HTuple();
                hv_ParamNames3DGrippingPointsOptional[0] = "min_z";
                hv_ParamNames3DGrippingPointsOptional[1] = "max_z";
                hv_ParamNames3DGrippingPointsOptional[2] = "normal_image_width";
                hv_ParamNames3DGrippingPointsOptional[3] = "normal_image_height";
                //All parameters
                hv_ParamNamesAll.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ParamNamesAll = new HTuple();
                    hv_ParamNamesAll = hv_ParamNamesAll.TupleConcat(hv_ParamNamesGeneral, hv_ParamNamesSegmentation, hv_ParamNamesDetectionOptional, hv_ParamNames3DGrippingPointsOptional, hv_ParamNamesPreprocessingOptional);
                }
                hv_ParamNames.Dispose();
                hv_ParamNames = new HTuple(hv_ParamNamesGeneral);
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("segmentation"))).TupleOr(new HTuple(hv_DLModelType.TupleEqual(
                    "3d_gripping_point_detection")))) != 0)
                {
                    //Extend ParamNames for models of type segmentation.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ParamNames = hv_ParamNames.TupleConcat(
                                hv_ParamNamesSegmentation);
                            hv_ParamNames.Dispose();
                            hv_ParamNames = ExpTmpLocalVar_ParamNames;
                        }
                    }
                }
                //
                //Check if legacy parameter exist.
                //Otherwise map it to the legal parameter.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                //
                //Check that all necessary parameters are included.
                //
                hv_KeysExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNames,
                    out hv_KeysExists);
                if ((int)(new HTuple(((((hv_KeysExists.TupleEqualElem(0))).TupleSum())).TupleGreater(
                    0))) != 0)
                {
                    for (hv_I = 0; (int)hv_I <= (int)(new HTuple(hv_KeysExists.TupleLength())); hv_I = (int)hv_I + 1)
                    {
                        hv_Exists.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Exists = hv_KeysExists.TupleSelect(
                                hv_I);
                        }
                        if ((int)(hv_Exists.TupleNot()) != 0)
                        {
                            throw new HalconException(("DLPreprocessParam needs the parameter: '" + (hv_ParamNames.TupleSelect(
                                hv_I))) + "'");
                        }
                    }
                }
                //
                //Check the keys provided.
                hv_InputKeys.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "keys", new HTuple(), out hv_InputKeys);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_InputKeys.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_Key.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Key = hv_InputKeys.TupleSelect(
                            hv_I);
                    }
                    hv_Value.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_Key, out hv_Value);
                    //Check that the key is known.
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_ParamNamesAll, hv_Key, out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                    {
                        throw new HalconException(("Unknown key for DLPreprocessParam: '" + (hv_InputKeys.TupleSelect(
                            hv_I))) + "'");

                        hv_CheckParams.Dispose();
                        hv_KeyExists.Dispose();
                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNames3DGrippingPointsOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IsInstanceSegmentation.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                    //Set expected values and types.
                    hv_ValidValues.Dispose();
                    hv_ValidValues = new HTuple();
                    hv_ValidTypes.Dispose();
                    hv_ValidTypes = new HTuple();
                    if ((int)(new HTuple(hv_Key.TupleEqual("normalization_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "all_channels";
                        hv_ValidValues[1] = "first_channel";
                        hv_ValidValues[2] = "constant_values";
                        hv_ValidValues[3] = "none";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("domain_handling"))) != 0)
                    {
                        if ((int)(new HTuple(hv_DLModelType.TupleEqual("anomaly_detection"))) != 0)
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                            hv_ValidValues[2] = "keep_domain";
                        }
                        else if ((int)(new HTuple(hv_DLModelType.TupleEqual("3d_gripping_point_detection"))) != 0)
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                            hv_ValidValues[2] = "keep_domain";
                        }
                        else
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                        }
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("model_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "3d_gripping_point_detection";
                        hv_ValidValues[1] = "anomaly_detection";
                        hv_ValidValues[2] = "classification";
                        hv_ValidValues[3] = "detection";
                        hv_ValidValues[4] = "gc_anomaly_detection";
                        hv_ValidValues[5] = "ocr_recognition";
                        hv_ValidValues[6] = "ocr_detection";
                        hv_ValidValues[7] = "segmentation";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("augmentation"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "true";
                        hv_ValidValues[1] = "false";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("set_background_id"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("class_ids_background"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    //Check that type is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        for (hv_V = 0; (int)hv_V <= (int)((new HTuple(hv_ValidTypes.TupleLength())) - 1); hv_V = (int)hv_V + 1)
                        {
                            hv_T.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_T = hv_ValidTypes.TupleSelect(
                                    hv_V);
                            }
                            if ((int)(new HTuple(hv_T.TupleEqual("int"))) != 0)
                            {
                                hv_IsInt.Dispose();
                                HOperatorSet.TupleIsInt(hv_Value, out hv_IsInt);
                                if ((int)(hv_IsInt.TupleNot()) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_ValidTypes = ("'" + hv_ValidTypes) + "'";
                                            hv_ValidTypes.Dispose();
                                            hv_ValidTypes = ExpTmpLocalVar_ValidTypes;
                                        }
                                    }
                                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleLess(
                                        2))) != 0)
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        hv_ValidTypesListing = new HTuple(hv_ValidTypes);
                                    }
                                    else
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_ValidTypesListing = ((((hv_ValidTypes.TupleSelectRange(
                                                0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 2))) + new HTuple(", ")) + (hv_ValidTypes.TupleSelect((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 1)))).TupleSum();
                                        }
                                    }
                                    throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid types are: ") + hv_ValidTypesListing) + ". The given value was '") + hv_Value) + "'.");

                                    hv_CheckParams.Dispose();
                                    hv_KeyExists.Dispose();
                                    hv_DLModelType.Dispose();
                                    hv_Exception.Dispose();
                                    hv_SupportedModelTypes.Dispose();
                                    hv_Index.Dispose();
                                    hv_ParamNamesGeneral.Dispose();
                                    hv_ParamNamesSegmentation.Dispose();
                                    hv_ParamNamesDetectionOptional.Dispose();
                                    hv_ParamNamesPreprocessingOptional.Dispose();
                                    hv_ParamNames3DGrippingPointsOptional.Dispose();
                                    hv_ParamNamesAll.Dispose();
                                    hv_ParamNames.Dispose();
                                    hv_KeysExists.Dispose();
                                    hv_I.Dispose();
                                    hv_Exists.Dispose();
                                    hv_InputKeys.Dispose();
                                    hv_Key.Dispose();
                                    hv_Value.Dispose();
                                    hv_Indices.Dispose();
                                    hv_ValidValues.Dispose();
                                    hv_ValidTypes.Dispose();
                                    hv_V.Dispose();
                                    hv_T.Dispose();
                                    hv_IsInt.Dispose();
                                    hv_ValidTypesListing.Dispose();
                                    hv_ValidValueListing.Dispose();
                                    hv_EmptyStrings.Dispose();
                                    hv_ImageRangeMinExists.Dispose();
                                    hv_ImageRangeMaxExists.Dispose();
                                    hv_ImageRangeMin.Dispose();
                                    hv_ImageRangeMax.Dispose();
                                    hv_IndexParam.Dispose();
                                    hv_SetBackgroundID.Dispose();
                                    hv_ClassIDsBackground.Dispose();
                                    hv_Intersection.Dispose();
                                    hv_IgnoreClassIDs.Dispose();
                                    hv_KnownClasses.Dispose();
                                    hv_IgnoreClassID.Dispose();
                                    hv_OptionalKeysExist.Dispose();
                                    hv_InstanceType.Dispose();
                                    hv_IsInstanceSegmentation.Dispose();
                                    hv_IgnoreDirection.Dispose();
                                    hv_ClassIDsNoOrientation.Dispose();
                                    hv_SemTypes.Dispose();

                                    return;
                                }
                            }
                            else
                            {
                                throw new HalconException("Internal error. Unknown valid type.");
                            }
                        }
                    }
                    //Check that value is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_ValidValues, hv_Value, out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ValidValues = ("'" + hv_ValidValues) + "'";
                                    hv_ValidValues.Dispose();
                                    hv_ValidValues = ExpTmpLocalVar_ValidValues;
                                }
                            }
                            if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleLess(
                                2))) != 0)
                            {
                                hv_ValidValueListing.Dispose();
                                hv_ValidValueListing = new HTuple(hv_ValidValues);
                            }
                            else
                            {
                                hv_EmptyStrings.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_EmptyStrings = HTuple.TupleGenConst(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 2, "");
                                }
                                hv_ValidValueListing.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ValidValueListing = ((((hv_ValidValues.TupleSelectRange(
                                        0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidValues.TupleLength()
                                        )) - 2))) + new HTuple(", ")) + (hv_EmptyStrings.TupleConcat(hv_ValidValues.TupleSelect(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 1))))).TupleSum();
                                }
                            }
                            throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid values are: ") + hv_ValidValueListing) + ". The given value was '") + hv_Value) + "'.");
                        }
                    }
                }
                //
                //Check the correct setting of ImageRangeMin and ImageRangeMax.
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("classification"))).TupleOr(
                    new HTuple(hv_DLModelType.TupleEqual("detection")))) != 0)
                {
                    //Check ImageRangeMin and ImageRangeMax.
                    hv_ImageRangeMinExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_min",
                        out hv_ImageRangeMinExists);
                    hv_ImageRangeMaxExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_max",
                        out hv_ImageRangeMaxExists);
                    //If they are present, check that they are set correctly.
                    if ((int)(hv_ImageRangeMinExists) != 0)
                    {
                        hv_ImageRangeMin.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                        if ((int)(new HTuple(hv_ImageRangeMin.TupleNotEqual(-127))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMin has to be -127.");
                        }
                    }
                    if ((int)(hv_ImageRangeMaxExists) != 0)
                    {
                        hv_ImageRangeMax.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                        if ((int)(new HTuple(hv_ImageRangeMax.TupleNotEqual(128))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMax has to be 128.");
                        }
                    }
                }
                //
                //Check segmentation specific parameters.
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("segmentation"))).TupleOr(new HTuple(hv_DLModelType.TupleEqual(
                    "3d_gripping_point_detection")))) != 0)
                {
                    //Check if detection specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesDetectionOptional.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for segmentation it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check 'set_background_id'.
                    hv_SetBackgroundID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                    if ((int)((new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))).TupleAnd(
                        new HTuple(hv_DLModelType.TupleEqual("3d_gripping_point_detection")))) != 0)
                    {
                        throw new HalconException(new HTuple(new HTuple("The preprocessing parameter '") + "set_background_id") + new HTuple("' should be set to [] for 3d_gripping_point_detection, as it is not used for this method."));
                    }
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    //Check 'class_ids_background'.
                    hv_ClassIDsBackground.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassIDsBackground);
                    if ((int)((new HTuple(hv_ClassIDsBackground.TupleNotEqual(new HTuple()))).TupleAnd(
                        new HTuple(hv_DLModelType.TupleEqual("3d_gripping_point_detection")))) != 0)
                    {
                        throw new HalconException(new HTuple(new HTuple("The preprocessing parameter '") + "class_ids_background") + new HTuple("' should be set to [] for 3d_gripping_point_detection, as it is not used for this method."));
                    }
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleNot()))).TupleOr((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleNot()))) != 0)
                    {
                        throw new HalconException("Both keys 'set_background_id' and 'class_ids_background' are required.");
                    }
                    //Check that 'class_ids_background' and 'set_background_id' are disjoint.
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Intersection.Dispose();
                        HOperatorSet.TupleIntersection(hv_SetBackgroundID, hv_ClassIDsBackground,
                            out hv_Intersection);
                        if ((int)(new HTuple(hv_Intersection.TupleLength())) != 0)
                        {
                            throw new HalconException("Class IDs in 'set_background_id' and 'class_ids_background' need to be disjoint.");
                        }
                    }
                    //Check 'ignore_class_ids'.
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                    if ((int)((new HTuple(hv_IgnoreClassIDs.TupleNotEqual(new HTuple()))).TupleAnd(
                        new HTuple(hv_DLModelType.TupleEqual("3d_gripping_point_detection")))) != 0)
                    {
                        throw new HalconException(new HTuple(new HTuple("The preprocessing parameter '") + "ignore_class_ids") + new HTuple("' should be set to [] for 3d_gripping_point_detection, as it is not used for this method."));
                    }
                    hv_KnownClasses.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KnownClasses = new HTuple();
                        hv_KnownClasses = hv_KnownClasses.TupleConcat(hv_SetBackgroundID, hv_ClassIDsBackground);
                    }
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_IgnoreClassIDs.TupleLength()
                        )) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_IgnoreClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreClassID = hv_IgnoreClassIDs.TupleSelect(
                                hv_I);
                        }
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_KnownClasses, hv_IgnoreClassID, out hv_Index);
                        if ((int)((new HTuple((new HTuple(hv_Index.TupleLength())).TupleGreater(
                            0))).TupleAnd(new HTuple(hv_Index.TupleNotEqual(-1)))) != 0)
                        {
                            throw new HalconException("The given 'ignore_class_ids' must not be included in the 'class_ids_background' or 'set_background_id'.");
                        }
                    }
                }
                else if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    //Check if segmentation specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesSegmentation,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesSegmentation.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for detection it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check optional parameters.
                    hv_OptionalKeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_OptionalKeysExist);
                    if ((int)(hv_OptionalKeysExist.TupleSelect(0)) != 0)
                    {
                        //Check 'instance_type'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceType.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                0), out hv_InstanceType);
                        }
                        if ((int)(new HTuple((new HTuple((((new HTuple("rectangle1")).TupleConcat(
                            "rectangle2")).TupleConcat("mask")).TupleFind(hv_InstanceType))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_type': " + hv_InstanceType) + new HTuple(", only 'rectangle1' and 'rectangle2' are allowed"));
                        }
                    }
                    //If instance_segmentation is set we might overwrite the instance_type for the preprocessing.
                    if ((int)(hv_OptionalKeysExist.TupleSelect(3)) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IsInstanceSegmentation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                3), out hv_IsInstanceSegmentation);
                        }
                        if ((int)(new HTuple((new HTuple(((((new HTuple(1)).TupleConcat(0)).TupleConcat(
                            "true")).TupleConcat("false")).TupleFind(hv_IsInstanceSegmentation))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_segmentation': " + hv_IsInstanceSegmentation) + new HTuple(", only true, false, 'true' and 'false' are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(1)) != 0)
                    {
                        //Check 'ignore_direction'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreDirection.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                1), out hv_IgnoreDirection);
                        }
                        if ((int)(new HTuple((new HTuple(((new HTuple(1)).TupleConcat(0)).TupleFind(
                            hv_IgnoreDirection))).TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'ignore_direction': " + hv_IgnoreDirection) + new HTuple(", only true and false are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(2)) != 0)
                    {
                        //Check 'class_ids_no_orientation'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassIDsNoOrientation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                2), out hv_ClassIDsNoOrientation);
                        }
                        hv_SemTypes.Dispose();
                        HOperatorSet.TupleSemTypeElem(hv_ClassIDsNoOrientation, out hv_SemTypes);
                        if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                            new HTuple(((((hv_SemTypes.TupleEqualElem("integer"))).TupleSum())).TupleNotEqual(
                            new HTuple(hv_ClassIDsNoOrientation.TupleLength()))))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only integers are allowed"));
                        }
                        else
                        {
                            if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                                new HTuple(((((hv_ClassIDsNoOrientation.TupleGreaterEqualElem(0))).TupleSum()
                                )).TupleNotEqual(new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                                ))))) != 0)
                            {
                                throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only non-negative integers are allowed"));
                            }
                        }
                    }
                }
                //

                hv_CheckParams.Dispose();
                hv_KeyExists.Dispose();
                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNames3DGrippingPointsOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_CheckParams.Dispose();
                hv_KeyExists.Dispose();
                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNames3DGrippingPointsOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Compute 3D normals. 
        private void compute_normals_xyz(HObject ho_x, HObject ho_y, HObject ho_z, out HObject ho_NXImage,
            out HObject ho_NYImage, out HObject ho_NZImage, HTuple hv_Smoothing)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_xScaled, ho_yScaled, ho_zScaled;
            HObject ho_xDiffRow, ho_xDiffCol, ho_yDiffRow, ho_yDiffCol;
            HObject ho_zDiffRow, ho_zDiffCol, ho_ImageResult, ho_ImageResult2;
            HObject ho_NXRaw, ho_NYRaw, ho_NZRaw, ho_NXSquare, ho_NYSquare;
            HObject ho_NZSquare, ho_ImageResult1, ho_SqrtImage;

            // Local control variables 

            HTuple hv_Factor = new HTuple(), hv_MaskRow = new HTuple();
            HTuple hv_MaskCol = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_NXImage);
            HOperatorSet.GenEmptyObj(out ho_NYImage);
            HOperatorSet.GenEmptyObj(out ho_NZImage);
            HOperatorSet.GenEmptyObj(out ho_xScaled);
            HOperatorSet.GenEmptyObj(out ho_yScaled);
            HOperatorSet.GenEmptyObj(out ho_zScaled);
            HOperatorSet.GenEmptyObj(out ho_xDiffRow);
            HOperatorSet.GenEmptyObj(out ho_xDiffCol);
            HOperatorSet.GenEmptyObj(out ho_yDiffRow);
            HOperatorSet.GenEmptyObj(out ho_yDiffCol);
            HOperatorSet.GenEmptyObj(out ho_zDiffRow);
            HOperatorSet.GenEmptyObj(out ho_zDiffCol);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);
            HOperatorSet.GenEmptyObj(out ho_ImageResult2);
            HOperatorSet.GenEmptyObj(out ho_NXRaw);
            HOperatorSet.GenEmptyObj(out ho_NYRaw);
            HOperatorSet.GenEmptyObj(out ho_NZRaw);
            HOperatorSet.GenEmptyObj(out ho_NXSquare);
            HOperatorSet.GenEmptyObj(out ho_NYSquare);
            HOperatorSet.GenEmptyObj(out ho_NZSquare);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);
            HOperatorSet.GenEmptyObj(out ho_SqrtImage);
            try
            {
                //For numerical reasons we scale the input data
                hv_Factor.Dispose();
                hv_Factor = 1e6;
                ho_xScaled.Dispose();
                HOperatorSet.ScaleImage(ho_x, out ho_xScaled, hv_Factor, 0);
                ho_yScaled.Dispose();
                HOperatorSet.ScaleImage(ho_y, out ho_yScaled, hv_Factor, 0);
                ho_zScaled.Dispose();
                HOperatorSet.ScaleImage(ho_z, out ho_zScaled, hv_Factor, 0);

                //Filter for diffs in row/col direction
                hv_MaskRow.Dispose();
                hv_MaskRow = new HTuple();
                hv_MaskRow[0] = 2;
                hv_MaskRow[1] = 1;
                hv_MaskRow[2] = 1.0;
                hv_MaskRow[3] = 1;
                hv_MaskRow[4] = -1;
                hv_MaskCol.Dispose();
                hv_MaskCol = new HTuple();
                hv_MaskCol[0] = 1;
                hv_MaskCol[1] = 2;
                hv_MaskCol[2] = 1.0;
                hv_MaskCol[3] = -1;
                hv_MaskCol[4] = 1;
                ho_xDiffRow.Dispose();
                HOperatorSet.ConvolImage(ho_xScaled, out ho_xDiffRow, hv_MaskRow, "continued");
                ho_xDiffCol.Dispose();
                HOperatorSet.ConvolImage(ho_xScaled, out ho_xDiffCol, hv_MaskCol, "continued");
                ho_yDiffRow.Dispose();
                HOperatorSet.ConvolImage(ho_yScaled, out ho_yDiffRow, hv_MaskRow, "continued");
                ho_yDiffCol.Dispose();
                HOperatorSet.ConvolImage(ho_yScaled, out ho_yDiffCol, hv_MaskCol, "continued");
                ho_zDiffRow.Dispose();
                HOperatorSet.ConvolImage(ho_zScaled, out ho_zDiffRow, hv_MaskRow, "continued");
                ho_zDiffCol.Dispose();
                HOperatorSet.ConvolImage(ho_zScaled, out ho_zDiffCol, hv_MaskCol, "continued");
                //
                //Calculate normal as cross product
                ho_ImageResult.Dispose();
                HOperatorSet.MultImage(ho_yDiffRow, ho_zDiffCol, out ho_ImageResult, 1.0, 0);
                ho_ImageResult2.Dispose();
                HOperatorSet.MultImage(ho_zDiffRow, ho_yDiffCol, out ho_ImageResult2, -1.0,
                    0);
                ho_NXRaw.Dispose();
                HOperatorSet.AddImage(ho_ImageResult, ho_ImageResult2, out ho_NXRaw, 1.0, 0);
                //
                ho_ImageResult.Dispose();
                HOperatorSet.MultImage(ho_xDiffRow, ho_zDiffCol, out ho_ImageResult, -1.0,
                    0);
                ho_ImageResult2.Dispose();
                HOperatorSet.MultImage(ho_zDiffRow, ho_xDiffCol, out ho_ImageResult2, 1.0,
                    0);
                ho_NYRaw.Dispose();
                HOperatorSet.AddImage(ho_ImageResult, ho_ImageResult2, out ho_NYRaw, 1.0, 0);
                //
                ho_ImageResult.Dispose();
                HOperatorSet.MultImage(ho_xDiffRow, ho_yDiffCol, out ho_ImageResult, 1.0, 0);
                ho_ImageResult2.Dispose();
                HOperatorSet.MultImage(ho_yDiffRow, ho_xDiffCol, out ho_ImageResult2, -1.0,
                    0);
                ho_NZRaw.Dispose();
                HOperatorSet.AddImage(ho_ImageResult, ho_ImageResult2, out ho_NZRaw, 1.0, 0);

                //Smooth
                //-> 5 is used as it is used in surface_normals_object_model_3d - 'xyz_mapping'
                if ((int)(hv_Smoothing) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MeanImage(ho_NXRaw, out ExpTmpOutVar_0, 5, 5);
                        ho_NXRaw.Dispose();
                        ho_NXRaw = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MeanImage(ho_NYRaw, out ExpTmpOutVar_0, 5, 5);
                        ho_NYRaw.Dispose();
                        ho_NYRaw = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MeanImage(ho_NZRaw, out ExpTmpOutVar_0, 5, 5);
                        ho_NZRaw.Dispose();
                        ho_NZRaw = ExpTmpOutVar_0;
                    }
                }

                //Normalize
                ho_NXSquare.Dispose();
                HOperatorSet.MultImage(ho_NXRaw, ho_NXRaw, out ho_NXSquare, 1.0, 0);
                ho_NYSquare.Dispose();
                HOperatorSet.MultImage(ho_NYRaw, ho_NYRaw, out ho_NYSquare, 1.0, 0);
                ho_NZSquare.Dispose();
                HOperatorSet.MultImage(ho_NZRaw, ho_NZRaw, out ho_NZSquare, 1.0, 0);
                ho_ImageResult1.Dispose();
                HOperatorSet.AddImage(ho_NXSquare, ho_NYSquare, out ho_ImageResult1, 1.0, 0);
                ho_ImageResult2.Dispose();
                HOperatorSet.AddImage(ho_ImageResult1, ho_NZSquare, out ho_ImageResult2, 1.0,
                    0);
                ho_SqrtImage.Dispose();
                HOperatorSet.SqrtImage(ho_ImageResult2, out ho_SqrtImage);
                //
                ho_NXImage.Dispose();
                HOperatorSet.DivImage(ho_NXRaw, ho_SqrtImage, out ho_NXImage, 1.0, 0);
                ho_NYImage.Dispose();
                HOperatorSet.DivImage(ho_NYRaw, ho_SqrtImage, out ho_NYImage, 1.0, 0);
                ho_NZImage.Dispose();
                HOperatorSet.DivImage(ho_NZRaw, ho_SqrtImage, out ho_NZImage, 1.0, 0);
                ho_xScaled.Dispose();
                ho_yScaled.Dispose();
                ho_zScaled.Dispose();
                ho_xDiffRow.Dispose();
                ho_xDiffCol.Dispose();
                ho_yDiffRow.Dispose();
                ho_yDiffCol.Dispose();
                ho_zDiffRow.Dispose();
                ho_zDiffCol.Dispose();
                ho_ImageResult.Dispose();
                ho_ImageResult2.Dispose();
                ho_NXRaw.Dispose();
                ho_NYRaw.Dispose();
                ho_NZRaw.Dispose();
                ho_NXSquare.Dispose();
                ho_NYSquare.Dispose();
                ho_NZSquare.Dispose();
                ho_ImageResult1.Dispose();
                ho_SqrtImage.Dispose();

                hv_Factor.Dispose();
                hv_MaskRow.Dispose();
                hv_MaskCol.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_xScaled.Dispose();
                ho_yScaled.Dispose();
                ho_zScaled.Dispose();
                ho_xDiffRow.Dispose();
                ho_xDiffCol.Dispose();
                ho_yDiffRow.Dispose();
                ho_yDiffCol.Dispose();
                ho_zDiffRow.Dispose();
                ho_zDiffCol.Dispose();
                ho_ImageResult.Dispose();
                ho_ImageResult2.Dispose();
                ho_NXRaw.Dispose();
                ho_NYRaw.Dispose();
                ho_NZRaw.Dispose();
                ho_NXSquare.Dispose();
                ho_NYSquare.Dispose();
                ho_NZSquare.Dispose();
                ho_ImageResult1.Dispose();
                ho_SqrtImage.Dispose();

                hv_Factor.Dispose();
                hv_MaskRow.Dispose();
                hv_MaskCol.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Tools / Geometry
        // Short Description: Convert the parameters of rectangles with format rectangle2 to the coordinates of its 4 corner-points. 
        private void convert_rect2_5to8param(HTuple hv_Row, HTuple hv_Col, HTuple hv_Length1,
            HTuple hv_Length2, HTuple hv_Phi, out HTuple hv_Row1, out HTuple hv_Col1, out HTuple hv_Row2,
            out HTuple hv_Col2, out HTuple hv_Row3, out HTuple hv_Col3, out HTuple hv_Row4,
            out HTuple hv_Col4)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Co1 = new HTuple(), hv_Co2 = new HTuple();
            HTuple hv_Si1 = new HTuple(), hv_Si2 = new HTuple();
            // Initialize local and output iconic variables 
            hv_Row1 = new HTuple();
            hv_Col1 = new HTuple();
            hv_Row2 = new HTuple();
            hv_Col2 = new HTuple();
            hv_Row3 = new HTuple();
            hv_Col3 = new HTuple();
            hv_Row4 = new HTuple();
            hv_Col4 = new HTuple();
            try
            {
                //This procedure takes the parameters for a rectangle of type 'rectangle2'
                //and returns the coordinates of the four corners.
                //
                hv_Co1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Co1 = (hv_Phi.TupleCos()
                        ) * hv_Length1;
                }
                hv_Co2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Co2 = (hv_Phi.TupleCos()
                        ) * hv_Length2;
                }
                hv_Si1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Si1 = (hv_Phi.TupleSin()
                        ) * hv_Length1;
                }
                hv_Si2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Si2 = (hv_Phi.TupleSin()
                        ) * hv_Length2;
                }

                hv_Col1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col1 = (hv_Co1 - hv_Si2) + hv_Col;
                }
                hv_Row1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row1 = ((-hv_Si1) - hv_Co2) + hv_Row;
                }
                hv_Col2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col2 = ((-hv_Co1) - hv_Si2) + hv_Col;
                }
                hv_Row2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row2 = (hv_Si1 - hv_Co2) + hv_Row;
                }
                hv_Col3.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col3 = ((-hv_Co1) + hv_Si2) + hv_Col;
                }
                hv_Row3.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row3 = (hv_Si1 + hv_Co2) + hv_Row;
                }
                hv_Col4.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col4 = (hv_Co1 + hv_Si2) + hv_Col;
                }
                hv_Row4.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row4 = ((-hv_Si1) + hv_Co2) + hv_Row;
                }


                hv_Co1.Dispose();
                hv_Co2.Dispose();
                hv_Si1.Dispose();
                hv_Si2.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Co1.Dispose();
                hv_Co2.Dispose();
                hv_Si1.Dispose();
                hv_Si2.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Tools / Geometry
        // Short Description: Convert for four-sided figures the coordinates of the 4 corner-points to the parameters of format rectangle2. 
        private void convert_rect2_8to5param(HTuple hv_Row1, HTuple hv_Col1, HTuple hv_Row2,
            HTuple hv_Col2, HTuple hv_Row3, HTuple hv_Col3, HTuple hv_Row4, HTuple hv_Col4,
            HTuple hv_ForceL1LargerL2, out HTuple hv_Row, out HTuple hv_Col, out HTuple hv_Length1,
            out HTuple hv_Length2, out HTuple hv_Phi)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Hor = new HTuple(), hv_Vert = new HTuple();
            HTuple hv_IdxSwap = new HTuple(), hv_Tmp = new HTuple();
            // Initialize local and output iconic variables 
            hv_Row = new HTuple();
            hv_Col = new HTuple();
            hv_Length1 = new HTuple();
            hv_Length2 = new HTuple();
            hv_Phi = new HTuple();
            try
            {
                //This procedure takes the corners of four-sided figures
                //and returns the parameters of type 'rectangle2'.
                //
                //Calculate center row and column.
                hv_Row.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row = (((hv_Row1 + hv_Row2) + hv_Row3) + hv_Row4) / 4.0;
                }
                hv_Col.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col = (((hv_Col1 + hv_Col2) + hv_Col3) + hv_Col4) / 4.0;
                }
                //Length1 and Length2.
                hv_Length1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length1 = (((((hv_Row1 - hv_Row2) * (hv_Row1 - hv_Row2)) + ((hv_Col1 - hv_Col2) * (hv_Col1 - hv_Col2)))).TupleSqrt()
                        ) / 2.0;
                }
                hv_Length2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length2 = (((((hv_Row2 - hv_Row3) * (hv_Row2 - hv_Row3)) + ((hv_Col2 - hv_Col3) * (hv_Col2 - hv_Col3)))).TupleSqrt()
                        ) / 2.0;
                }
                //Calculate the angle phi.
                hv_Hor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Hor = hv_Col1 - hv_Col2;
                }
                hv_Vert.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Vert = hv_Row2 - hv_Row1;
                }
                if ((int)(hv_ForceL1LargerL2) != 0)
                {
                    //Swap length1 and length2 if necessary.
                    hv_IdxSwap.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_IdxSwap = ((((hv_Length2 - hv_Length1)).TupleGreaterElem(
                            1e-9))).TupleFind(1);
                    }
                    if ((int)(new HTuple(hv_IdxSwap.TupleNotEqual(-1))) != 0)
                    {
                        hv_Tmp.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Tmp = hv_Length1.TupleSelect(
                                hv_IdxSwap);
                        }
                        if (hv_Length1 == null)
                            hv_Length1 = new HTuple();
                        hv_Length1[hv_IdxSwap] = hv_Length2.TupleSelect(hv_IdxSwap);
                        if (hv_Length2 == null)
                            hv_Length2 = new HTuple();
                        hv_Length2[hv_IdxSwap] = hv_Tmp;
                        if (hv_Hor == null)
                            hv_Hor = new HTuple();
                        hv_Hor[hv_IdxSwap] = (hv_Col2.TupleSelect(hv_IdxSwap)) - (hv_Col3.TupleSelect(
                            hv_IdxSwap));
                        if (hv_Vert == null)
                            hv_Vert = new HTuple();
                        hv_Vert[hv_IdxSwap] = (hv_Row3.TupleSelect(hv_IdxSwap)) - (hv_Row2.TupleSelect(
                            hv_IdxSwap));
                    }
                }
                hv_Phi.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Phi = hv_Vert.TupleAtan2(
                        hv_Hor);
                }
                //

                hv_Hor.Dispose();
                hv_Vert.Dispose();
                hv_IdxSwap.Dispose();
                hv_Tmp.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Hor.Dispose();
                hv_Vert.Dispose();
                hv_IdxSwap.Dispose();
                hv_Tmp.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Create a dictionary with preprocessing parameters. 
        public void create_dl_preprocess_param(HTuple hv_DLModelType, HTuple hv_ImageWidth,
            HTuple hv_ImageHeight, HTuple hv_ImageNumChannels, HTuple hv_ImageRangeMin,
            HTuple hv_ImageRangeMax, HTuple hv_NormalizationType, HTuple hv_DomainHandling,
            HTuple hv_IgnoreClassIDs, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground,
            HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {



            // Local control variables 

            HTuple hv_GenParamNames = new HTuple(), hv_GenParamIndex = new HTuple();
            HTuple hv_GenParamValue = new HTuple(), hv_KeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IsInstanceSegmentation = new HTuple();
            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            try
            {
                //
                //This procedure creates a dictionary with all parameters needed for preprocessing.
                //
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.CreateDict(out hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "model_type", hv_DLModelType);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_width", hv_ImageWidth);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_height", hv_ImageHeight);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", hv_ImageNumChannels);
                if ((int)(new HTuple(hv_ImageRangeMin.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", -127);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", hv_ImageRangeMin);
                }
                if ((int)(new HTuple(hv_ImageRangeMax.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", 128);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", hv_ImageRangeMax);
                }
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                //Replace possible legacy parameters.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "domain_handling", hv_DomainHandling);
                //
                //Set segmentation and '3d_gripping_point_detection' parameters.
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("segmentation"))).TupleOr(new HTuple(hv_DLModelType.TupleEqual(
                    "3d_gripping_point_detection")))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", hv_IgnoreClassIDs);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "set_background_id", hv_SetBackgroundID);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "class_ids_background", hv_ClassIDsBackground);
                }
                //
                //Set default values of generic parameters.
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "augmentation", "false");
                //
                //Set generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamNames.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamNames);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamNames.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GenParamValue.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, hv_GenParamNames.TupleSelect(hv_GenParamIndex),
                                out hv_GenParamValue);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, hv_GenParamNames.TupleSelect(
                                hv_GenParamIndex), hv_GenParamValue);
                        }
                    }
                }
                //
                //Set necessary default values.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    hv_KeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", ((new HTuple("instance_type")).TupleConcat(
                        "ignore_direction")).TupleConcat("instance_segmentation"), out hv_KeysExist);
                    if ((int)(((hv_KeysExist.TupleSelect(0))).TupleNot()) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "rectangle1");
                    }
                    //Set default for 'ignore_direction' only if instance_type is 'rectangle2'.
                    hv_InstanceType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_type", out hv_InstanceType);
                    if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                        ((hv_KeysExist.TupleSelect(1))).TupleNot())) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_direction", 0);
                    }
                    //In case of instance_segmentation we overwrite the instance_type to mask.
                    if ((int)(hv_KeysExist.TupleSelect(2)) != 0)
                    {
                        hv_IsInstanceSegmentation.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_segmentation",
                            out hv_IsInstanceSegmentation);
                        if ((int)(new HTuple((new HTuple(((((new HTuple(1)).TupleConcat(0)).TupleConcat(
                            "true")).TupleConcat("false")).TupleFind(hv_IsInstanceSegmentation))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_segmentation': " + hv_IsInstanceSegmentation) + new HTuple(", only true and false are allowed"));
                        }
                        if ((int)((new HTuple(hv_IsInstanceSegmentation.TupleEqual("true"))).TupleOr(
                            new HTuple(hv_IsInstanceSegmentation.TupleEqual(1)))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "mask");
                        }
                    }
                }
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Create a dictionary with the preprocessing parameters based on a given DL model. 
        public void create_dl_preprocess_param_from_model(HTuple hv_DLModelHandle, HTuple hv_NormalizationType,
            HTuple hv_DomainHandling, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground,
            HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ModelType = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ImageNumChannels = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_InstanceType = new HTuple();
            HTuple hv_IsInstanceSegmentation = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple();
            HTuple hv_GenParam_COPY_INP_TMP = new HTuple(hv_GenParam);

            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            try
            {
                //
                //This procedure creates a dictionary with all parameters needed for preprocessing
                //according to a model provided through DLModelHandle.
                //
                //Get the relevant model parameters.
                hv_ModelType.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "type", out hv_ModelType);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_max", out hv_ImageRangeMax);
                hv_IgnoreClassIDs.Dispose();
                hv_IgnoreClassIDs = new HTuple();
                //
                //Get model specific parameters.
                if ((int)((new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))).TupleOr(
                    new HTuple(hv_ModelType.TupleEqual("gc_anomaly_detection")))) != 0)
                {
                    //No specific parameters for both anomaly detection
                    //and Global Context Anomaly Detection model types.
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("classification"))) != 0)
                {
                    //No classification specific parameters.
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("detection"))) != 0)
                {
                    //Get detection specific parameters.
                    //If GenParam has not been created yet, create it to add new generic parameters.
                    if ((int)(new HTuple((new HTuple(hv_GenParam_COPY_INP_TMP.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        hv_GenParam_COPY_INP_TMP.Dispose();
                        HOperatorSet.CreateDict(out hv_GenParam_COPY_INP_TMP);
                    }
                    //Add instance_type.
                    hv_InstanceType.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "instance_type", out hv_InstanceType);
                    //If the model can do instance segmentation, the preprocessing instance_type
                    //needs to be 'mask'.
                    hv_IsInstanceSegmentation.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "instance_segmentation", out hv_IsInstanceSegmentation);
                    if ((int)(new HTuple(hv_IsInstanceSegmentation.TupleEqual("true"))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "instance_type", "mask");
                    }
                    else
                    {
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "instance_type", hv_InstanceType);
                    }
                    //For instance_type 'rectangle2', add the boolean ignore_direction and class IDs without orientation.
                    if ((int)(new HTuple(hv_InstanceType.TupleEqual("rectangle2"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        HOperatorSet.GetDlModelParam(hv_DLModelHandle, "ignore_direction", out hv_IgnoreDirection);
                        if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("true"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "ignore_direction",
                                1);
                        }
                        else if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("false"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "ignore_direction",
                                0);
                        }
                        hv_ClassIDsNoOrientation.Dispose();
                        HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids_no_orientation",
                            out hv_ClassIDsNoOrientation);
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "class_ids_no_orientation",
                            hv_ClassIDsNoOrientation);
                    }
                }
                else if ((int)((new HTuple(hv_ModelType.TupleEqual("ocr_detection"))).TupleOr(
                    new HTuple(hv_ModelType.TupleEqual("ocr_recognition")))) != 0)
                {
                    //No ocr specific parameters.
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Get segmentation specific parameters.
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "ignore_class_ids", out hv_IgnoreClassIDs);
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("3d_gripping_point_detection"))) != 0)
                {
                    //The input image is expected to be a single channel image.
                    hv_ImageNumChannels.Dispose();
                    hv_ImageNumChannels = 1;
                }
                else
                {
                    throw new HalconException(("Current model type is not supported: \"" + hv_ModelType) + "\"");
                }
                //
                //Create the dictionary with the preprocessing parameters returned by this procedure.
                hv_DLPreprocessParam.Dispose();
                create_dl_preprocess_param(hv_ModelType, hv_ImageWidth, hv_ImageHeight, hv_ImageNumChannels,
                    hv_ImageRangeMin, hv_ImageRangeMax, hv_NormalizationType, hv_DomainHandling,
                    hv_IgnoreClassIDs, hv_SetBackgroundID, hv_ClassIDsBackground, hv_GenParam_COPY_INP_TMP,
                    out hv_DLPreprocessParam);
                //

                hv_GenParam_COPY_INP_TMP.Dispose();
                hv_ModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_GenParam_COPY_INP_TMP.Dispose();
                hv_ModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Crops a given image object based on the given domain handling. 
        private void crop_dl_sample_image(HObject ho_Domain, HTuple hv_DLSample, HTuple hv_Key,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho___Tmp_Obj_0 = null;

            // Local control variables 

            HTuple hv_KeyExists = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv___Tmp_Ctrl_Dict_Init_0 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho___Tmp_Obj_0);
            try
            {
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", hv_Key, out hv_KeyExists);
                if ((int)(hv_KeyExists) != 0)
                {
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_0);
                    HOperatorSet.SetDictTuple(hv___Tmp_Ctrl_Dict_Init_0, "comp", "crop_domain");
                    if ((int)(((hv_DLPreprocessParam.TupleConcat(hv___Tmp_Ctrl_Dict_Init_0))).TupleTestEqualDictItem(
                        "domain_handling", "comp")) != 0)
                    {
                        hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                        HOperatorSet.SmallestRectangle1(ho_Domain, out hv_Row1, out hv_Column1,
                            out hv_Row2, out hv_Column2);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho___Tmp_Obj_0.Dispose();
                            HOperatorSet.CropPart(hv_DLSample.TupleGetDictObject(hv_Key), out ho___Tmp_Obj_0,
                                hv_Row1, hv_Column1, (hv_Column2 - hv_Column1) + 1, (hv_Row2 - hv_Row1) + 1);
                        }
                        HOperatorSet.SetDictObject(ho___Tmp_Obj_0, hv_DLSample, hv_Key);
                    }
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv___Tmp_Ctrl_Dict_Init_0 = HTuple.TupleConstant(
                            "HNULL");
                    }
                }
                ho___Tmp_Obj_0.Dispose();

                hv_KeyExists.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho___Tmp_Obj_0.Dispose();

                hv_KeyExists.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection and Instance Segmentation
        // Short Description: Filter the instance segmentation masks of a DL sample based on a given selection. 
        private void filter_dl_sample_instance_segmentation_masks(HTuple hv_DLSample,
            HTuple hv_BBoxSelectionMask)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_EmptyMasks = null, ho_Masks = null;

            // Local control variables 

            HTuple hv_MaskKeyExists = new HTuple(), hv_Indices = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_EmptyMasks);
            HOperatorSet.GenEmptyObj(out ho_Masks);
            try
            {
                hv_MaskKeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "mask", out hv_MaskKeyExists);
                if ((int)(hv_MaskKeyExists) != 0)
                {
                    //Only if masks exist (-> instance segmentation).
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_BBoxSelectionMask, 1, out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                    {
                        //We define here that this case will result in an empty object value
                        //for the mask key. Another option would be to remove the
                        //key 'mask'. However, this would be an unwanted big change in the dictionary.
                        ho_EmptyMasks.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_EmptyMasks);
                        HOperatorSet.SetDictObject(ho_EmptyMasks, hv_DLSample, "mask");
                    }
                    else
                    {
                        ho_Masks.Dispose();
                        HOperatorSet.GetDictObject(out ho_Masks, hv_DLSample, "mask");
                        //Remove all unused masks.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.SelectObj(ho_Masks, out ExpTmpOutVar_0, hv_Indices + 1);
                            ho_Masks.Dispose();
                            ho_Masks = ExpTmpOutVar_0;
                        }
                        HOperatorSet.SetDictObject(ho_Masks, hv_DLSample, "mask");
                    }
                }
                ho_EmptyMasks.Dispose();
                ho_Masks.Dispose();

                hv_MaskKeyExists.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_EmptyMasks.Dispose();
                ho_Masks.Dispose();

                hv_MaskKeyExists.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate ground truth characters if they don't exist and words to characters mapping. 
        private void gen_dl_ocr_detection_gt_chars(HTuple hv_DLSampleTargets, HTuple hv_DLSample,
            HTuple hv_ScaleWidth, HTuple hv_ScaleHeight, out HTupleVector/*{eTupleVector,Dim=1}*/ hvec_WordsCharsMapping)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CharBoxIndex = new HTuple(), hv_WordLengths = new HTuple();
            HTuple hv_J = new HTuple(), hv_Start = new HTuple(), hv_End = new HTuple();
            HTuple hv_SplitRow = new HTuple(), hv_SplitColumn = new HTuple();
            HTuple hv_SplitPhi = new HTuple(), hv_SplitLength1 = new HTuple();
            HTuple hv_SplitLength2 = new HTuple(), hv_CharsIds = new HTuple();
            HTuple hv_EmptyWordStrings = new HTuple();
            // Initialize local and output iconic variables 
            hvec_WordsCharsMapping = new HTupleVector(1);
            try
            {
                hvec_WordsCharsMapping[0] = new HTupleVector(new HTuple());
                if ((int)(new HTuple((new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Check if chars GT exist otherwise generate them.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CharBoxIndex.Dispose();
                        HOperatorSet.TupleFindFirst(hv_DLSample.TupleGetDictTuple("bbox_label_id"),
                            1, out hv_CharBoxIndex);
                    }
                    if ((int)(new HTuple(hv_CharBoxIndex.TupleEqual(-1))) != 0)
                    {
                        hv_WordLengths.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WordLengths = ((hv_DLSample.TupleGetDictTuple(
                                "word"))).TupleStrlen();
                        }
                        hvec_WordsCharsMapping[(new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                            )) - 1] = new HTupleVector(new HTuple());
                        for (hv_J = 0; (int)hv_J <= (int)((new HTuple(((hv_DLSample.TupleGetDictTuple(
                            "bbox_label_id"))).TupleLength())) - 1); hv_J = (int)hv_J + 1)
                        {
                            //For each word box
                            if ((int)(new HTuple(((((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleSelect(
                                hv_J))).TupleEqual(0))) != 0)
                            {
                                if ((int)(new HTuple(((hv_WordLengths.TupleSelect(hv_J))).TupleNotEqual(
                                    0))) != 0)
                                {
                                    hv_Start.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_Start = new HTuple(((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_label_id"))).TupleLength());
                                    }
                                    hv_End.Dispose();
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_End = ((new HTuple(((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_label_id"))).TupleLength())) - 1) + (hv_WordLengths.TupleSelect(
                                            hv_J));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hvec_WordsCharsMapping[hv_J] = dh.Add(new HTupleVector(HTuple.TupleGenSequence(
                                            hv_Start, hv_End, 1)));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_SplitRow.Dispose(); hv_SplitColumn.Dispose(); hv_SplitPhi.Dispose(); hv_SplitLength1.Dispose(); hv_SplitLength2.Dispose();
                                        split_rectangle2(((hv_DLSample.TupleGetDictTuple("bbox_row"))).TupleSelect(
                                            hv_J), ((hv_DLSample.TupleGetDictTuple("bbox_col"))).TupleSelect(
                                            hv_J), ((hv_DLSample.TupleGetDictTuple("bbox_phi"))).TupleSelect(
                                            hv_J), ((hv_DLSample.TupleGetDictTuple("bbox_length1"))).TupleSelect(
                                            hv_J), ((hv_DLSample.TupleGetDictTuple("bbox_length2"))).TupleSelect(
                                            hv_J), hv_WordLengths.TupleSelect(hv_J), out hv_SplitRow, out hv_SplitColumn,
                                            out hv_SplitPhi, out hv_SplitLength1, out hv_SplitLength2);
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_CharsIds.Dispose();
                                        HOperatorSet.TupleGenConst(hv_WordLengths.TupleSelect(hv_J), 1, out hv_CharsIds);
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hv_EmptyWordStrings.Dispose();
                                        HOperatorSet.TupleGenConst(hv_WordLengths.TupleSelect(hv_J), "",
                                            out hv_EmptyWordStrings);
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "bbox_label_id", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_label_id"))).TupleConcat(hv_CharsIds));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "bbox_row", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_row"))).TupleConcat(hv_SplitRow));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "bbox_col", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_col"))).TupleConcat(hv_SplitColumn));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "bbox_phi", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_phi"))).TupleConcat(hv_SplitPhi));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "bbox_length1", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_length1"))).TupleConcat(hv_SplitLength1 * hv_ScaleWidth));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "bbox_length2", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "bbox_length2"))).TupleConcat(hv_SplitLength2 * hv_ScaleHeight));
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.SetDictTuple(hv_DLSampleTargets, "word", ((hv_DLSampleTargets.TupleGetDictTuple(
                                            "word"))).TupleConcat(hv_EmptyWordStrings));
                                    }
                                }
                                else
                                {
                                    throw new HalconException(((("Sample with image id " + (hv_DLSample.TupleGetDictTuple(
                                        "image_id"))) + " is not valid. The word bounding box at index ") + hv_J) + " has an empty string as the ground truth. This is not allowed. Please assign a word label to every word bounding box.");
                                }
                            }
                        }
                    }
                    else
                    {
                        hvec_WordsCharsMapping.Dispose();
                        gen_words_chars_mapping(hv_DLSample, out hvec_WordsCharsMapping);
                    }
                }

                hv_CharBoxIndex.Dispose();
                hv_WordLengths.Dispose();
                hv_J.Dispose();
                hv_Start.Dispose();
                hv_End.Dispose();
                hv_SplitRow.Dispose();
                hv_SplitColumn.Dispose();
                hv_SplitPhi.Dispose();
                hv_SplitLength1.Dispose();
                hv_SplitLength2.Dispose();
                hv_CharsIds.Dispose();
                hv_EmptyWordStrings.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_CharBoxIndex.Dispose();
                hv_WordLengths.Dispose();
                hv_J.Dispose();
                hv_Start.Dispose();
                hv_End.Dispose();
                hv_SplitRow.Dispose();
                hv_SplitColumn.Dispose();
                hv_SplitPhi.Dispose();
                hv_SplitLength1.Dispose();
                hv_SplitLength2.Dispose();
                hv_CharsIds.Dispose();
                hv_EmptyWordStrings.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate target link score map for ocr detection training. 
        private void gen_dl_ocr_detection_gt_link_map(out HObject ho_GtLinkMap, HTuple hv_ImageWidth,
            HTuple hv_ImageHeight, HTuple hv_DLSampleTargets, HTupleVector/*{eTupleVector,Dim=1}*/ hvec_WordToCharVec,
            HTuple hv_Alpha)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Lines = null, ho_Line = null, ho_LineDilated = null;

            // Local control variables 

            HTuple hv_InitImage = new HTuple(), hv_CRow = new HTuple();
            HTuple hv_CCol = new HTuple(), hv_DiameterC = new HTuple();
            HTuple hv_IndexW = new HTuple(), hv_CharBoxIndices = new HTuple();
            HTuple hv_CharCRows = new HTuple(), hv_CharCCols = new HTuple();
            HTuple hv_CharDistToWordCenter = new HTuple(), hv_ExtremeCharIndex = new HTuple();
            HTuple hv_DistToExtreme = new HTuple(), hv_CharIndexSorted = new HTuple();
            HTuple hv_Box1Idx = new HTuple(), hv_Box2Idx = new HTuple();
            HTuple hv_Diameter1 = new HTuple(), hv_Diameter2 = new HTuple();
            HTuple hv_DilationRadius = new HTuple(), hv_NumLines = new HTuple();
            HTuple hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_GtLinkMap);
            HOperatorSet.GenEmptyObj(out ho_Lines);
            HOperatorSet.GenEmptyObj(out ho_Line);
            HOperatorSet.GenEmptyObj(out ho_LineDilated);
            try
            {
                ho_GtLinkMap.Dispose();
                HOperatorSet.GenImageConst(out ho_GtLinkMap, "real", hv_ImageWidth, hv_ImageHeight);
                hv_InitImage.Dispose();
                HOperatorSet.GetSystem("init_new_image", out hv_InitImage);
                if ((int)(new HTuple(hv_InitImage.TupleEqual("false"))) != 0)
                {
                    HOperatorSet.OverpaintRegion(ho_GtLinkMap, ho_GtLinkMap, 0.0, "fill");
                }
                //Compute box centers.
                hv_CRow.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CRow = hv_DLSampleTargets.TupleGetDictTuple(
                        "bbox_row");
                }
                hv_CCol.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_CCol = hv_DLSampleTargets.TupleGetDictTuple(
                        "bbox_col");
                }
                hv_DiameterC.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DiameterC = 2 * (((hv_DLSampleTargets.TupleGetDictTuple(
                        "bbox_length1"))).TupleHypot(hv_DLSampleTargets.TupleGetDictTuple("bbox_length2")));
                }
                //Loop over word boxes.
                for (hv_IndexW = 0; (int)hv_IndexW <= (int)((new HTuple(((hv_DLSampleTargets.TupleGetDictTuple(
                    "bbox_label_id"))).TupleLength())) - 1); hv_IndexW = (int)hv_IndexW + 1)
                {
                    //For each word box
                    if ((int)(new HTuple(((((hv_DLSampleTargets.TupleGetDictTuple("bbox_label_id"))).TupleSelect(
                        hv_IndexW))).TupleEqual(0))) != 0)
                    {
                        hv_CharBoxIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CharBoxIndices = new HTuple(hvec_WordToCharVec[hv_IndexW].T);
                        }
                        if ((int)(new HTuple((new HTuple(hv_CharBoxIndices.TupleLength())).TupleEqual(
                            0))) != 0)
                        {
                            continue;
                        }
                        else if ((int)(new HTuple((new HTuple(hv_CharBoxIndices.TupleLength()
                            )).TupleEqual(1))) != 0)
                        {
                            //Generate a dot in the char center.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_Lines.Dispose();
                                HOperatorSet.GenCircle(out ho_Lines, hv_CRow.TupleSelect(hv_CharBoxIndices),
                                    hv_CCol.TupleSelect(hv_CharBoxIndices), ((((0.5 * hv_Alpha) * (hv_DiameterC.TupleSelect(
                                    hv_CharBoxIndices)))).TupleRound()) + 0.5);
                            }
                        }
                        else
                        {
                            //Generate link lines between chars.
                            hv_CharCRows.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_CharCRows = hv_CRow.TupleSelect(
                                    hv_CharBoxIndices);
                            }
                            hv_CharCCols.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_CharCCols = hv_CCol.TupleSelect(
                                    hv_CharBoxIndices);
                            }
                            //Sort the char boxes within the word.
                            hv_CharDistToWordCenter.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_CharDistToWordCenter = ((hv_CharCRows - (hv_CRow.TupleSelect(
                                    hv_IndexW)))).TupleHypot(hv_CharCCols - (hv_CCol.TupleSelect(hv_IndexW)));
                            }
                            hv_ExtremeCharIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ExtremeCharIndex = (new HTuple(hv_CharDistToWordCenter.TupleSortIndex()
                                    )).TupleSelect((new HTuple(hv_CharDistToWordCenter.TupleLength())) - 1);
                            }
                            hv_DistToExtreme.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DistToExtreme = ((hv_CharCRows - (hv_CharCRows.TupleSelect(
                                    hv_ExtremeCharIndex)))).TupleHypot(hv_CharCCols - (hv_CharCCols.TupleSelect(
                                    hv_ExtremeCharIndex)));
                            }
                            hv_CharIndexSorted.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_CharIndexSorted = hv_DistToExtreme.TupleSortIndex()
                                    ;
                            }
                            //Get the indices of adjacent characters.
                            hv_Box1Idx.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Box1Idx = hv_CharIndexSorted.TupleSelectRange(
                                    0, (new HTuple(hv_CharIndexSorted.TupleLength())) - 2);
                            }
                            hv_Box2Idx.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Box2Idx = hv_CharIndexSorted.TupleSelectRange(
                                    1, (new HTuple(hv_CharIndexSorted.TupleLength())) - 1);
                            }
                            //Generate link lines between each pair of adjacent characters.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_Lines.Dispose();
                                HOperatorSet.GenRegionLine(out ho_Lines, hv_CharCRows.TupleSelect(hv_Box1Idx),
                                    hv_CharCCols.TupleSelect(hv_Box1Idx), hv_CharCRows.TupleSelect(hv_Box2Idx),
                                    hv_CharCCols.TupleSelect(hv_Box2Idx));
                            }
                            //Dilate the lines by 0.5/1.5/2.5/... pixels, such that the line thickness is approximately Alpha*mean(D1, D2)
                            hv_Diameter1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Diameter1 = hv_DiameterC.TupleSelect(
                                    hv_CharBoxIndices.TupleSelect(hv_Box1Idx));
                            }
                            hv_Diameter2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Diameter2 = hv_DiameterC.TupleSelect(
                                    hv_CharBoxIndices.TupleSelect(hv_Box2Idx));
                            }
                            hv_DilationRadius.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DilationRadius = ((((0.25 * hv_Alpha) * (hv_Diameter1 + hv_Diameter2))).TupleRound()
                                    ) + 0.5;
                            }
                            //dilation_circle only accepts a single radius, so we need to loop over the lines.
                            hv_NumLines.Dispose();
                            HOperatorSet.CountObj(ho_Lines, out hv_NumLines);
                            HTuple end_val39 = hv_NumLines;
                            HTuple step_val39 = 1;
                            for (hv_Index = 1; hv_Index.Continue(end_val39, step_val39); hv_Index = hv_Index.TupleAdd(step_val39))
                            {
                                ho_Line.Dispose();
                                HOperatorSet.SelectObj(ho_Lines, out ho_Line, hv_Index);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_LineDilated.Dispose();
                                    HOperatorSet.DilationCircle(ho_Line, out ho_LineDilated, hv_DilationRadius.TupleSelect(
                                        hv_Index - 1));
                                }
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Lines, ho_LineDilated, out ExpTmpOutVar_0,
                                        hv_Index);
                                    ho_Lines.Dispose();
                                    ho_Lines = ExpTmpOutVar_0;
                                }
                            }
                        }
                        HOperatorSet.OverpaintRegion(ho_GtLinkMap, ho_Lines, 1.0, "fill");
                    }
                }
                ho_Lines.Dispose();
                ho_Line.Dispose();
                ho_LineDilated.Dispose();

                hv_InitImage.Dispose();
                hv_CRow.Dispose();
                hv_CCol.Dispose();
                hv_DiameterC.Dispose();
                hv_IndexW.Dispose();
                hv_CharBoxIndices.Dispose();
                hv_CharCRows.Dispose();
                hv_CharCCols.Dispose();
                hv_CharDistToWordCenter.Dispose();
                hv_ExtremeCharIndex.Dispose();
                hv_DistToExtreme.Dispose();
                hv_CharIndexSorted.Dispose();
                hv_Box1Idx.Dispose();
                hv_Box2Idx.Dispose();
                hv_Diameter1.Dispose();
                hv_Diameter2.Dispose();
                hv_DilationRadius.Dispose();
                hv_NumLines.Dispose();
                hv_Index.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Lines.Dispose();
                ho_Line.Dispose();
                ho_LineDilated.Dispose();

                hv_InitImage.Dispose();
                hv_CRow.Dispose();
                hv_CCol.Dispose();
                hv_DiameterC.Dispose();
                hv_IndexW.Dispose();
                hv_CharBoxIndices.Dispose();
                hv_CharCRows.Dispose();
                hv_CharCCols.Dispose();
                hv_CharDistToWordCenter.Dispose();
                hv_ExtremeCharIndex.Dispose();
                hv_DistToExtreme.Dispose();
                hv_CharIndexSorted.Dispose();
                hv_Box1Idx.Dispose();
                hv_Box2Idx.Dispose();
                hv_Diameter1.Dispose();
                hv_Diameter2.Dispose();
                hv_DilationRadius.Dispose();
                hv_NumLines.Dispose();
                hv_Index.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate target orientation score maps for ocr detection training. 
        private void gen_dl_ocr_detection_gt_orientation_map(out HObject ho_GtOrientationMaps,
            HTuple hv_ImageWidth, HTuple hv_ImageHeight, HTuple hv_DLSample)
        {



            // Local iconic variables 

            HObject ho_GtOrientationSin, ho_GtOrientationCos;
            HObject ho_Region = null;

            // Local control variables 

            HTuple hv_InitImage = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_Phi = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_GtOrientationMaps);
            HOperatorSet.GenEmptyObj(out ho_GtOrientationSin);
            HOperatorSet.GenEmptyObj(out ho_GtOrientationCos);
            HOperatorSet.GenEmptyObj(out ho_Region);
            try
            {
                ho_GtOrientationSin.Dispose();
                HOperatorSet.GenImageConst(out ho_GtOrientationSin, "real", hv_ImageWidth,
                    hv_ImageHeight);
                ho_GtOrientationCos.Dispose();
                HOperatorSet.GenImageConst(out ho_GtOrientationCos, "real", hv_ImageWidth,
                    hv_ImageHeight);
                hv_InitImage.Dispose();
                HOperatorSet.GetSystem("init_new_image", out hv_InitImage);
                if ((int)(new HTuple(hv_InitImage.TupleEqual("false"))) != 0)
                {
                    HOperatorSet.OverpaintRegion(ho_GtOrientationSin, ho_GtOrientationSin, 0.0,
                        "fill");
                    HOperatorSet.OverpaintRegion(ho_GtOrientationCos, ho_GtOrientationCos, 0.0,
                        "fill");
                }
                if ((int)(new HTuple((new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Process char boxes
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices.Dispose();
                        HOperatorSet.TupleFind(hv_DLSample.TupleGetDictTuple("bbox_label_id"), 1,
                            out hv_Indices);
                    }
                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        hv_Phi.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Phi = hv_DLSample.TupleGetDictTuple(
                                "bbox_phi");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Region.Dispose();
                            HOperatorSet.GenRectangle2(out ho_Region, ((hv_DLSample.TupleGetDictTuple(
                                "bbox_row"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_col"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_phi"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_length1"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_length2"))).TupleSelect(hv_Indices));
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.OverpaintRegion(ho_GtOrientationSin, ho_Region, ((hv_Phi.TupleSelect(
                                hv_Indices))).TupleSin(), "fill");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.OverpaintRegion(ho_GtOrientationCos, ho_Region, ((hv_Phi.TupleSelect(
                                hv_Indices))).TupleCos(), "fill");
                        }
                    }
                }
                ho_GtOrientationMaps.Dispose();
                HOperatorSet.Compose2(ho_GtOrientationSin, ho_GtOrientationCos, out ho_GtOrientationMaps
                    );
                ho_GtOrientationSin.Dispose();
                ho_GtOrientationCos.Dispose();
                ho_Region.Dispose();

                hv_InitImage.Dispose();
                hv_Indices.Dispose();
                hv_Phi.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_GtOrientationSin.Dispose();
                ho_GtOrientationCos.Dispose();
                ho_Region.Dispose();

                hv_InitImage.Dispose();
                hv_Indices.Dispose();
                hv_Phi.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate target text score map for ocr detection training. 
        private void gen_dl_ocr_detection_gt_score_map(out HObject ho_TargetText, HTuple hv_DLSample,
            HTuple hv_BoxCutoff, HTuple hv_RenderCutoff, HTuple hv_ImageWidth, HTuple hv_ImageHeight)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ExtendedRectangle = null;

            // Local control variables 

            HTuple hv_InitImage = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Sigma1 = new HTuple(), hv_Sigma2 = new HTuple();
            HTuple hv_ExtendedLength1 = new HTuple(), hv_ExtendedLength2 = new HTuple();
            HTuple hv_Rows = new HTuple(), hv_Columns = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_HomMat2D = new HTuple(), hv_DistRow = new HTuple();
            HTuple hv_DistCol = new HTuple(), hv_ScaledGaussian = new HTuple();
            HTuple hv_Grayval = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TargetText);
            HOperatorSet.GenEmptyObj(out ho_ExtendedRectangle);
            try
            {
                ho_TargetText.Dispose();
                HOperatorSet.GenImageConst(out ho_TargetText, "real", hv_ImageWidth, hv_ImageHeight);
                hv_InitImage.Dispose();
                HOperatorSet.GetSystem("init_new_image", out hv_InitImage);
                if ((int)(new HTuple(hv_InitImage.TupleEqual("false"))) != 0)
                {
                    HOperatorSet.OverpaintRegion(ho_TargetText, ho_TargetText, 0.0, "fill");
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(((hv_DLSample.TupleGetDictTuple(
                    "bbox_label_id"))).TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    //For each char box
                    if ((int)((new HTuple(((((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleSelect(
                        hv_Index))).TupleEqual(1))).TupleAnd(new HTuple(hv_BoxCutoff.TupleNotEqual(
                        0)))) != 0)
                    {
                        //Compute the sigma of an unnormalized normal distribution, such that
                        //a certain threshold value is reached at the interval of a certain size.
                        hv_Sigma1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Sigma1 = (((hv_DLSample.TupleGetDictTuple(
                                "bbox_length1"))).TupleSelect(hv_Index)) * (((-0.5 / (hv_BoxCutoff.TupleLog()
                                ))).TupleSqrt());
                        }
                        hv_Sigma2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Sigma2 = (((hv_DLSample.TupleGetDictTuple(
                                "bbox_length2"))).TupleSelect(hv_Index)) * (((-0.5 / (hv_BoxCutoff.TupleLog()
                                ))).TupleSqrt());
                        }
                        if ((int)((new HTuple((new HTuple(hv_Sigma1.TupleNotEqual(0))).TupleAnd(
                            new HTuple(hv_Sigma2.TupleNotEqual(0))))).TupleAnd(new HTuple(hv_RenderCutoff.TupleNotEqual(
                            0)))) != 0)
                        {
                            //Compute the radius of an unnormalized normal distribution,
                            //where a certain threshold value is reached at the end.
                            hv_ExtendedLength1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ExtendedLength1 = hv_Sigma1 * (((-2 * (hv_RenderCutoff.TupleLog()
                                    ))).TupleSqrt());
                            }
                            hv_ExtendedLength2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ExtendedLength2 = hv_Sigma2 * (((-2 * (hv_RenderCutoff.TupleLog()
                                    ))).TupleSqrt());
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_ExtendedRectangle.Dispose();
                                HOperatorSet.GenRectangle2(out ho_ExtendedRectangle, ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_row"))).TupleSelect(hv_Index), ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_col"))).TupleSelect(hv_Index), ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_phi"))).TupleSelect(hv_Index), hv_ExtendedLength1, hv_ExtendedLength2);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ClipRegion(ho_ExtendedRectangle, out ExpTmpOutVar_0, 0,
                                    0, hv_ImageHeight - 1, hv_ImageWidth - 1);
                                ho_ExtendedRectangle.Dispose();
                                ho_ExtendedRectangle = ExpTmpOutVar_0;
                            }
                            hv_Rows.Dispose(); hv_Columns.Dispose();
                            HOperatorSet.GetRegionPoints(ho_ExtendedRectangle, out hv_Rows, out hv_Columns);
                            //Verify that the bounding box has an area to plot a gaussian
                            hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                            HOperatorSet.AreaCenter(ho_ExtendedRectangle, out hv_Area, out hv_Row,
                                out hv_Column);
                            if ((int)(new HTuple(hv_Area.TupleGreater(1))) != 0)
                            {
                                hv_HomMat2D.Dispose();
                                HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HTuple ExpTmpOutVar_0;
                                    HOperatorSet.HomMat2dTranslate(hv_HomMat2D, -(((hv_DLSample.TupleGetDictTuple(
                                        "bbox_row"))).TupleSelect(hv_Index)), -(((hv_DLSample.TupleGetDictTuple(
                                        "bbox_col"))).TupleSelect(hv_Index)), out ExpTmpOutVar_0);
                                    hv_HomMat2D.Dispose();
                                    hv_HomMat2D = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HTuple ExpTmpOutVar_0;
                                    HOperatorSet.HomMat2dRotate(hv_HomMat2D, -(((hv_DLSample.TupleGetDictTuple(
                                        "bbox_phi"))).TupleSelect(hv_Index)), 0, 0, out ExpTmpOutVar_0);
                                    hv_HomMat2D.Dispose();
                                    hv_HomMat2D = ExpTmpOutVar_0;
                                }
                                hv_DistRow.Dispose(); hv_DistCol.Dispose();
                                HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_Rows, hv_Columns, out hv_DistRow,
                                    out hv_DistCol);
                                hv_ScaledGaussian.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ScaledGaussian = ((-0.5 * (((hv_DistCol * hv_DistCol) / (hv_Sigma1 * hv_Sigma1)) + ((hv_DistRow * hv_DistRow) / (hv_Sigma2 * hv_Sigma2))))).TupleExp()
                                        ;
                                }
                                hv_Grayval.Dispose();
                                HOperatorSet.GetGrayval(ho_TargetText, hv_Rows, hv_Columns, out hv_Grayval);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.SetGrayval(ho_TargetText, hv_Rows, hv_Columns, hv_ScaledGaussian.TupleMax2(
                                        hv_Grayval));
                                }
                            }
                        }
                    }
                }
                ho_ExtendedRectangle.Dispose();

                hv_InitImage.Dispose();
                hv_Index.Dispose();
                hv_Sigma1.Dispose();
                hv_Sigma2.Dispose();
                hv_ExtendedLength1.Dispose();
                hv_ExtendedLength2.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_HomMat2D.Dispose();
                hv_DistRow.Dispose();
                hv_DistCol.Dispose();
                hv_ScaledGaussian.Dispose();
                hv_Grayval.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ExtendedRectangle.Dispose();

                hv_InitImage.Dispose();
                hv_Index.Dispose();
                hv_Sigma1.Dispose();
                hv_Sigma2.Dispose();
                hv_ExtendedLength1.Dispose();
                hv_ExtendedLength2.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_HomMat2D.Dispose();
                hv_DistRow.Dispose();
                hv_DistCol.Dispose();
                hv_ScaledGaussian.Dispose();
                hv_Grayval.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Preprocess dl samples and generate targets and weights for ocr detection training. 
        private void gen_dl_ocr_detection_targets(HTuple hv_DLSampleOriginal, HTuple hv_DLPreprocessParam)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TargetText, ho_TargetLink, ho_TargetOrientation;
            HObject ho_TargetWeightText, ho_TargetWeightLink, ho_WeightedCharScore;
            HObject ho_TargetWeightOrientation, ho_OriginalDomain, ho_Image = null;
            HObject ho_DomainWeight = null, ho_Domain = null, ho_TargetOrientationOut = null;
            HObject ho_TargetWeightOrientationOut = null, ho_TargetOrientationChannel = null;
            HObject ho_TargetWeightOrientationChannel = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_Stride = new HTuple(), hv_ScaleHeight = new HTuple();
            HTuple hv_ScaleWidth = new HTuple(), hv_BoxCutoff = new HTuple();
            HTuple hv_RenderCutoff = new HTuple(), hv_Alpha = new HTuple();
            HTuple hv_WSWeightRenderThreshold = new HTuple(), hv_LinkZeroWeightRadius = new HTuple();
            HTuple hv_Confidence = new HTuple(), hv_ScoreMapsWidth = new HTuple();
            HTuple hv_ScoreMapsHeight = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple(), hv_HomMat2DScale = new HTuple();
            HTuple hv_DLSampleTargets = new HTuple(), hv_OriginalDomainArea = new HTuple();
            HTuple hv__ = new HTuple(), hv_OriginalWidth = new HTuple();
            HTuple hv_OriginalHeight = new HTuple(), hv_IsOriginalDomainFull = new HTuple();
            HTuple hv_ChannelIdx = new HTuple(), hv___Tmp_Ctrl_0 = new HTuple();
            HTuple hv___Tmp_Ctrl_1 = new HTuple();

            HTupleVector hvec_WordsCharsMapping = new HTupleVector(1);
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TargetText);
            HOperatorSet.GenEmptyObj(out ho_TargetLink);
            HOperatorSet.GenEmptyObj(out ho_TargetOrientation);
            HOperatorSet.GenEmptyObj(out ho_TargetWeightText);
            HOperatorSet.GenEmptyObj(out ho_TargetWeightLink);
            HOperatorSet.GenEmptyObj(out ho_WeightedCharScore);
            HOperatorSet.GenEmptyObj(out ho_TargetWeightOrientation);
            HOperatorSet.GenEmptyObj(out ho_OriginalDomain);
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_DomainWeight);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            HOperatorSet.GenEmptyObj(out ho_TargetOrientationOut);
            HOperatorSet.GenEmptyObj(out ho_TargetWeightOrientationOut);
            HOperatorSet.GenEmptyObj(out ho_TargetOrientationChannel);
            HOperatorSet.GenEmptyObj(out ho_TargetWeightOrientationChannel);
            try
            {
                check_dl_preprocess_param(hv_DLPreprocessParam);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_Stride.Dispose();
                hv_Stride = 2;
                //Parameters used in the fallback weak supervision case.
                //They make the the uniformly sized char boxes a bit smaller, as we can expect a spacing between the characters.
                hv_ScaleHeight.Dispose();
                hv_ScaleHeight = 0.9;
                hv_ScaleWidth.Dispose();
                hv_ScaleWidth = 0.8;
                //Parameters relevant to plot the gaussian blobs in the score map.
                hv_BoxCutoff.Dispose();
                hv_BoxCutoff = 0.3;
                hv_RenderCutoff.Dispose();
                hv_RenderCutoff = 0.01;
                //Parameter used to determine the dilation of lines in link map.
                hv_Alpha.Dispose();
                hv_Alpha = 0.1;
                //Parameter used to determine the dilation radius of word boxes in the weight score map.
                hv_WSWeightRenderThreshold.Dispose();
                hv_WSWeightRenderThreshold = 0.05;
                //Parameter represents the dilation radius of word lines in the weight link map.
                hv_LinkZeroWeightRadius.Dispose();
                hv_LinkZeroWeightRadius = 2.5;
                //Confidence is here only a place holder for the fallback weak supervision case.
                hv_Confidence.Dispose();
                hv_Confidence = 1.0;
                if ((int)(new HTuple(hv_Stride.TupleEqual(0))) != 0)
                {
                    throw new HalconException("Stride must be greater than 0.");
                }
                //Calculate the size of score maps.
                hv_ScoreMapsWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScoreMapsWidth = hv_ImageWidth / hv_Stride;
                }
                hv_ScoreMapsHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScoreMapsHeight = hv_ImageHeight / hv_Stride;
                }
                //Copy DLSample to maintain the original bounding boxes dimensions.
                hv_DLSample.Dispose();
                HOperatorSet.CopyDict(hv_DLSampleOriginal, new HTuple(), new HTuple(), out hv_DLSample);
                //Preprocess bounding boxes to match targets dimensions.
                hv_HomMat2DIdentity.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HomMat2DScale.Dispose();
                    HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, 1.0 / hv_Stride, 1.0 / hv_Stride,
                        0, 0, out hv_HomMat2DScale);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv___Tmp_Ctrl_0.Dispose(); hv___Tmp_Ctrl_1.Dispose();
                    HOperatorSet.AffineTransPoint2d(hv_HomMat2DScale, hv_DLSample.TupleGetDictTuple(
                        "bbox_col"), hv_DLSample.TupleGetDictTuple("bbox_row"), out hv___Tmp_Ctrl_0,
                        out hv___Tmp_Ctrl_1);
                }
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row", hv___Tmp_Ctrl_1);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col", hv___Tmp_Ctrl_0);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length1", (hv_DLSample.TupleGetDictTuple(
                        "bbox_length1")) / hv_Stride);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length2", (hv_DLSample.TupleGetDictTuple(
                        "bbox_length2")) / hv_Stride);
                }
                hv_DLSampleTargets.Dispose();
                HOperatorSet.CopyDict(hv_DLSample, new HTuple(), new HTuple(), out hv_DLSampleTargets);
                hvec_WordsCharsMapping.Dispose();
                gen_dl_ocr_detection_gt_chars(hv_DLSampleTargets, hv_DLSample, hv_ScaleWidth,
                    hv_ScaleHeight, out hvec_WordsCharsMapping);
                //Generate target maps from WordRegions and CharBoxes.
                ho_TargetText.Dispose();
                gen_dl_ocr_detection_gt_score_map(out ho_TargetText, hv_DLSampleTargets, hv_BoxCutoff,
                    hv_RenderCutoff, hv_ScoreMapsWidth, hv_ScoreMapsHeight);
                ho_TargetLink.Dispose();
                gen_dl_ocr_detection_gt_link_map(out ho_TargetLink, hv_ScoreMapsWidth, hv_ScoreMapsHeight,
                    hv_DLSampleTargets, hvec_WordsCharsMapping, hv_Alpha);
                ho_TargetOrientation.Dispose();
                gen_dl_ocr_detection_gt_orientation_map(out ho_TargetOrientation, hv_ScoreMapsWidth,
                    hv_ScoreMapsHeight, hv_DLSampleTargets);
                //Generate weight maps from WordRegions and CharBoxes.
                ho_TargetWeightText.Dispose();
                gen_dl_ocr_detection_weight_score_map(out ho_TargetWeightText, hv_ScoreMapsWidth,
                    hv_ScoreMapsHeight, hv_DLSampleTargets, hv_BoxCutoff, hv_WSWeightRenderThreshold,
                    hv_Confidence);
                ho_TargetWeightLink.Dispose();
                gen_dl_ocr_detection_weight_link_map(ho_TargetLink, ho_TargetWeightText, out ho_TargetWeightLink,
                    hv_LinkZeroWeightRadius);
                ho_WeightedCharScore.Dispose();
                HOperatorSet.MultImage(ho_TargetText, ho_TargetWeightText, out ho_WeightedCharScore,
                    1, 0);
                ho_TargetWeightOrientation.Dispose();
                gen_dl_ocr_detection_weight_orientation_map(ho_WeightedCharScore, out ho_TargetWeightOrientation,
                    hv_DLSampleTargets);
                //Take account of the image domain in DLSampleOriginal.
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_OriginalDomain.Dispose();
                    HOperatorSet.GetDomain(hv_DLSampleOriginal.TupleGetDictObject("image"), out ho_OriginalDomain
                        );
                }
                hv_OriginalDomainArea.Dispose(); hv__.Dispose(); hv__.Dispose();
                HOperatorSet.AreaCenter(ho_OriginalDomain, out hv_OriginalDomainArea, out hv__,
                    out hv__);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_OriginalWidth.Dispose(); hv_OriginalHeight.Dispose();
                    HOperatorSet.GetImageSize(hv_DLSampleOriginal.TupleGetDictObject("image"),
                        out hv_OriginalWidth, out hv_OriginalHeight);
                }
                hv_IsOriginalDomainFull.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IsOriginalDomainFull = new HTuple(hv_OriginalDomainArea.TupleEqual(
                        hv_OriginalWidth * hv_OriginalHeight));
                }
                if ((int)(hv_IsOriginalDomainFull.TupleNot()) != 0)
                {
                    //Calculate the domain weight.
                    ho_Image.Dispose();
                    HOperatorSet.GenImageConst(out ho_Image, "real", hv_OriginalWidth, hv_OriginalHeight);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ChangeDomain(ho_Image, ho_OriginalDomain, out ExpTmpOutVar_0
                            );
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    ho_DomainWeight.Dispose();
                    HOperatorSet.ZoomImageSize(ho_Image, out ho_DomainWeight, hv_ScoreMapsWidth,
                        hv_ScoreMapsHeight, "constant");
                    ho_Domain.Dispose();
                    HOperatorSet.GetDomain(ho_DomainWeight, out ho_Domain);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_DomainWeight, out ExpTmpOutVar_0);
                        ho_DomainWeight.Dispose();
                        ho_DomainWeight = ExpTmpOutVar_0;
                    }
                    HOperatorSet.OverpaintRegion(ho_DomainWeight, ho_DomainWeight, 0.0, "fill");
                    HOperatorSet.OverpaintRegion(ho_DomainWeight, ho_Domain, 1.0, "fill");
                    //Apply the domain weight.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MultImage(ho_DomainWeight, ho_TargetText, out ExpTmpOutVar_0,
                            1, 0);
                        ho_TargetText.Dispose();
                        ho_TargetText = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MultImage(ho_DomainWeight, ho_TargetLink, out ExpTmpOutVar_0,
                            1, 0);
                        ho_TargetLink.Dispose();
                        ho_TargetLink = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MultImage(ho_DomainWeight, ho_TargetWeightText, out ExpTmpOutVar_0,
                            1, 0);
                        ho_TargetWeightText.Dispose();
                        ho_TargetWeightText = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MultImage(ho_DomainWeight, ho_TargetWeightLink, out ExpTmpOutVar_0,
                            1, 0);
                        ho_TargetWeightLink.Dispose();
                        ho_TargetWeightLink = ExpTmpOutVar_0;
                    }
                    ho_TargetOrientationOut.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_TargetOrientationOut);
                    ho_TargetWeightOrientationOut.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_TargetWeightOrientationOut);
                    for (hv_ChannelIdx = 1; (int)hv_ChannelIdx <= 2; hv_ChannelIdx = (int)hv_ChannelIdx + 1)
                    {
                        ho_TargetOrientationChannel.Dispose();
                        HOperatorSet.AccessChannel(ho_TargetOrientation, out ho_TargetOrientationChannel,
                            hv_ChannelIdx);
                        ho_TargetWeightOrientationChannel.Dispose();
                        HOperatorSet.AccessChannel(ho_TargetWeightOrientation, out ho_TargetWeightOrientationChannel,
                            hv_ChannelIdx);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.MultImage(ho_DomainWeight, ho_TargetOrientationChannel, out ExpTmpOutVar_0,
                                1, 0);
                            ho_TargetOrientationChannel.Dispose();
                            ho_TargetOrientationChannel = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.MultImage(ho_DomainWeight, ho_TargetWeightOrientationChannel,
                                out ExpTmpOutVar_0, 1, 0);
                            ho_TargetWeightOrientationChannel.Dispose();
                            ho_TargetWeightOrientationChannel = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.AppendChannel(ho_TargetOrientationOut, ho_TargetOrientationChannel,
                                out ExpTmpOutVar_0);
                            ho_TargetOrientationOut.Dispose();
                            ho_TargetOrientationOut = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.AppendChannel(ho_TargetWeightOrientationOut, ho_TargetWeightOrientationChannel,
                                out ExpTmpOutVar_0);
                            ho_TargetWeightOrientationOut.Dispose();
                            ho_TargetWeightOrientationOut = ExpTmpOutVar_0;
                        }
                    }
                    ho_TargetOrientation.Dispose();
                    ho_TargetOrientation = new HObject(ho_TargetOrientationOut);
                    ho_TargetWeightOrientation.Dispose();
                    ho_TargetWeightOrientation = new HObject(ho_TargetWeightOrientationOut);
                }
                //Set targets in output sample.
                HOperatorSet.SetDictObject(ho_TargetText, hv_DLSampleOriginal, "target_text");
                HOperatorSet.SetDictObject(ho_TargetLink, hv_DLSampleOriginal, "target_link");
                HOperatorSet.SetDictObject(ho_TargetOrientation, hv_DLSampleOriginal, "target_orientation");
                HOperatorSet.SetDictObject(ho_TargetWeightText, hv_DLSampleOriginal, "target_weight_text");
                HOperatorSet.SetDictObject(ho_TargetWeightLink, hv_DLSampleOriginal, "target_weight_link");
                HOperatorSet.SetDictObject(ho_TargetWeightOrientation, hv_DLSampleOriginal,
                    "target_weight_orientation");
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_TargetText.Dispose();
                ho_TargetLink.Dispose();
                ho_TargetOrientation.Dispose();
                ho_TargetWeightText.Dispose();
                ho_TargetWeightLink.Dispose();
                ho_WeightedCharScore.Dispose();
                ho_TargetWeightOrientation.Dispose();
                ho_OriginalDomain.Dispose();
                ho_Image.Dispose();
                ho_DomainWeight.Dispose();
                ho_Domain.Dispose();
                ho_TargetOrientationOut.Dispose();
                ho_TargetWeightOrientationOut.Dispose();
                ho_TargetOrientationChannel.Dispose();
                ho_TargetWeightOrientationChannel.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_Stride.Dispose();
                hv_ScaleHeight.Dispose();
                hv_ScaleWidth.Dispose();
                hv_BoxCutoff.Dispose();
                hv_RenderCutoff.Dispose();
                hv_Alpha.Dispose();
                hv_WSWeightRenderThreshold.Dispose();
                hv_LinkZeroWeightRadius.Dispose();
                hv_Confidence.Dispose();
                hv_ScoreMapsWidth.Dispose();
                hv_ScoreMapsHeight.Dispose();
                hv_DLSample.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv_DLSampleTargets.Dispose();
                hv_OriginalDomainArea.Dispose();
                hv__.Dispose();
                hv_OriginalWidth.Dispose();
                hv_OriginalHeight.Dispose();
                hv_IsOriginalDomainFull.Dispose();
                hv_ChannelIdx.Dispose();
                hv___Tmp_Ctrl_0.Dispose();
                hv___Tmp_Ctrl_1.Dispose();
                hvec_WordsCharsMapping.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate link score map weight for ocr detection training. 
        private void gen_dl_ocr_detection_weight_link_map(HObject ho_LinkMap, HObject ho_TargetWeight,
            out HObject ho_TargetWeightLink, HTuple hv_LinkZeroWeightRadius)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_LinkRegion = null, ho_RegionDilation = null;
            HObject ho_RegionComplement = null, ho_RegionUnion = null, ho_RegionBorder = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TargetWeightLink);
            HOperatorSet.GenEmptyObj(out ho_LinkRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionComplement);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionBorder);
            try
            {
                if ((int)(new HTuple(hv_LinkZeroWeightRadius.TupleGreater(0))) != 0)
                {
                    //Set zero weight around the link regions.
                    ho_LinkRegion.Dispose();
                    HOperatorSet.Threshold(ho_LinkMap, out ho_LinkRegion, 0.01, "max");
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationCircle(ho_LinkRegion, out ho_RegionDilation, hv_LinkZeroWeightRadius);
                    ho_RegionComplement.Dispose();
                    HOperatorSet.Complement(ho_RegionDilation, out ho_RegionComplement);
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_TargetWeight, out hv_Width, out hv_Height);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ClipRegion(ho_RegionComplement, out ExpTmpOutVar_0, 0, 0, hv_Height - 1,
                            hv_Width - 1);
                        ho_RegionComplement.Dispose();
                        ho_RegionComplement = ExpTmpOutVar_0;
                    }
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union2(ho_LinkRegion, ho_RegionComplement, out ho_RegionUnion
                        );
                    ho_RegionBorder.Dispose();
                    HOperatorSet.Complement(ho_RegionUnion, out ho_RegionBorder);
                    ho_TargetWeightLink.Dispose();
                    HOperatorSet.PaintRegion(ho_RegionBorder, ho_TargetWeight, out ho_TargetWeightLink,
                        0, "fill");
                }
                else
                {
                    //Just copy the original weight map.
                    ho_TargetWeightLink.Dispose();
                    HOperatorSet.CopyObj(ho_TargetWeight, out ho_TargetWeightLink, 1, 1);
                }
                ho_LinkRegion.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionComplement.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionBorder.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_LinkRegion.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionComplement.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionBorder.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate orientation score map weight for ocr detection training. 
        private void gen_dl_ocr_detection_weight_orientation_map(HObject ho_InitialWeight,
            out HObject ho_OrientationTargetWeight, HTuple hv_DLSample)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_CharRegions = null, ho_CharRegion = null;
            HObject ho_BackgroundRegion = null;

            // Local control variables 

            HTuple hv_Indices = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_OrientationTargetWeight);
            HOperatorSet.GenEmptyObj(out ho_CharRegions);
            HOperatorSet.GenEmptyObj(out ho_CharRegion);
            HOperatorSet.GenEmptyObj(out ho_BackgroundRegion);
            try
            {
                //Inside the valid regions, the inital weight is set to the initial weight.
                ho_OrientationTargetWeight.Dispose();
                HOperatorSet.CopyImage(ho_InitialWeight, out ho_OrientationTargetWeight);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FullDomain(ho_OrientationTargetWeight, out ExpTmpOutVar_0);
                    ho_OrientationTargetWeight.Dispose();
                    ho_OrientationTargetWeight = ExpTmpOutVar_0;
                }
                //Set orientation weight to 0 outside the valid regions.
                if ((int)(new HTuple((new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Process char boxes
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices.Dispose();
                        HOperatorSet.TupleFind(hv_DLSample.TupleGetDictTuple("bbox_label_id"), 1,
                            out hv_Indices);
                    }
                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_CharRegions.Dispose();
                            HOperatorSet.GenRectangle2(out ho_CharRegions, ((hv_DLSample.TupleGetDictTuple(
                                "bbox_row"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_col"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_phi"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_length1"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_length2"))).TupleSelect(hv_Indices));
                        }
                        ho_CharRegion.Dispose();
                        HOperatorSet.Union1(ho_CharRegions, out ho_CharRegion);
                        ho_BackgroundRegion.Dispose();
                        HOperatorSet.Complement(ho_CharRegion, out ho_BackgroundRegion);
                        HOperatorSet.OverpaintRegion(ho_OrientationTargetWeight, ho_BackgroundRegion,
                            0, "fill");
                    }
                }
                //We need two channels: for Sin and Cos
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Compose2(ho_OrientationTargetWeight, ho_OrientationTargetWeight,
                        out ExpTmpOutVar_0);
                    ho_OrientationTargetWeight.Dispose();
                    ho_OrientationTargetWeight = ExpTmpOutVar_0;
                }
                ho_CharRegions.Dispose();
                ho_CharRegion.Dispose();
                ho_BackgroundRegion.Dispose();

                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_CharRegions.Dispose();
                ho_CharRegion.Dispose();
                ho_BackgroundRegion.Dispose();

                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate text score map weight for ocr detection training. 
        private void gen_dl_ocr_detection_weight_score_map(out HObject ho_TargetWeightText,
            HTuple hv_ImageWidth, HTuple hv_ImageHeight, HTuple hv_DLSample, HTuple hv_BoxCutoff,
            HTuple hv_WSWeightRenderThreshold, HTuple hv_Confidence)
        {



            // Local iconic variables 

            HObject ho_IgnoreRegion = null, ho_WordRegion = null;
            HObject ho_WordRegionDilated = null;

            // Local control variables 

            HTuple hv_Indices = new HTuple(), hv_WordIndex = new HTuple();
            HTuple hv_SigmaL2 = new HTuple(), hv_WordLength2Ext = new HTuple();
            HTuple hv_DilationRadius = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_TargetWeightText);
            HOperatorSet.GenEmptyObj(out ho_IgnoreRegion);
            HOperatorSet.GenEmptyObj(out ho_WordRegion);
            HOperatorSet.GenEmptyObj(out ho_WordRegionDilated);
            try
            {
                ho_TargetWeightText.Dispose();
                HOperatorSet.GenImageConst(out ho_TargetWeightText, "real", hv_ImageWidth,
                    hv_ImageHeight);
                HOperatorSet.OverpaintRegion(ho_TargetWeightText, ho_TargetWeightText, 1.0,
                    "fill");
                if ((int)(new HTuple((new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Process ignore boxes
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices.Dispose();
                        HOperatorSet.TupleFind(hv_DLSample.TupleGetDictTuple("bbox_label_id"), 2,
                            out hv_Indices);
                    }
                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_IgnoreRegion.Dispose();
                            HOperatorSet.GenRectangle2(out ho_IgnoreRegion, ((hv_DLSample.TupleGetDictTuple(
                                "bbox_row"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_col"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_phi"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_length1"))).TupleSelect(hv_Indices), ((hv_DLSample.TupleGetDictTuple(
                                "bbox_length2"))).TupleSelect(hv_Indices));
                        }
                        HOperatorSet.OverpaintRegion(ho_TargetWeightText, ho_IgnoreRegion, 0.0,
                            "fill");
                    }
                    for (hv_WordIndex = 0; (int)hv_WordIndex <= (int)((new HTuple(((hv_DLSample.TupleGetDictTuple(
                        "bbox_label_id"))).TupleLength())) - 1); hv_WordIndex = (int)hv_WordIndex + 1)
                    {
                        //For each word box
                        if ((int)(new HTuple(((((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleSelect(
                            hv_WordIndex))).TupleEqual(0))) != 0)
                        {
                            if ((int)((new HTuple((new HTuple(hv_BoxCutoff.TupleEqual(0))).TupleOr(
                                new HTuple(hv_WSWeightRenderThreshold.TupleEqual(0))))).TupleNot()
                                ) != 0)
                            {
                                hv_SigmaL2.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_SigmaL2 = (((hv_DLSample.TupleGetDictTuple(
                                        "bbox_length2"))).TupleSelect(hv_WordIndex)) * (((-0.5 / (hv_BoxCutoff.TupleLog()
                                        ))).TupleSqrt());
                                }
                                hv_WordLength2Ext.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_WordLength2Ext = hv_SigmaL2 * (((-2 * (hv_WSWeightRenderThreshold.TupleLog()
                                        ))).TupleSqrt());
                                }
                                hv_DilationRadius.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_DilationRadius = hv_WordLength2Ext - (((hv_DLSample.TupleGetDictTuple(
                                        "bbox_length2"))).TupleSelect(hv_WordIndex));
                                }
                            }
                            else
                            {
                                hv_DilationRadius.Dispose();
                                hv_DilationRadius = 0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_WordRegion.Dispose();
                                HOperatorSet.GenRectangle2(out ho_WordRegion, ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_row"))).TupleSelect(hv_WordIndex), ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_col"))).TupleSelect(hv_WordIndex), ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_phi"))).TupleSelect(hv_WordIndex), ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_length1"))).TupleSelect(hv_WordIndex), ((hv_DLSample.TupleGetDictTuple(
                                    "bbox_length2"))).TupleSelect(hv_WordIndex));
                            }
                            //Slightly enlarge the weight region to suppress halos at the box borders.
                            if ((int)(new HTuple(hv_DilationRadius.TupleGreaterEqual(0.5))) != 0)
                            {
                                ho_WordRegionDilated.Dispose();
                                HOperatorSet.DilationCircle(ho_WordRegion, out ho_WordRegionDilated,
                                    hv_DilationRadius);
                            }
                            else
                            {
                                ho_WordRegionDilated.Dispose();
                                ho_WordRegionDilated = new HObject(ho_WordRegion);
                            }
                            //Set the confidence as weight for the word region.
                            HOperatorSet.OverpaintRegion(ho_TargetWeightText, ho_WordRegionDilated,
                                hv_Confidence, "fill");
                        }
                    }
                }
                ho_IgnoreRegion.Dispose();
                ho_WordRegion.Dispose();
                ho_WordRegionDilated.Dispose();

                hv_Indices.Dispose();
                hv_WordIndex.Dispose();
                hv_SigmaL2.Dispose();
                hv_WordLength2Ext.Dispose();
                hv_DilationRadius.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_IgnoreRegion.Dispose();
                ho_WordRegion.Dispose();
                ho_WordRegionDilated.Dispose();

                hv_Indices.Dispose();
                hv_WordIndex.Dispose();
                hv_SigmaL2.Dispose();
                hv_WordLength2Ext.Dispose();
                hv_DilationRadius.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Store the given images in a tuple of dictionaries DLSamples. 
        public void gen_dl_samples_from_images(HObject ho_Images, out HTuple hv_DLSampleBatch)
        {



            // Local iconic variables 

            HObject ho_Image = null;

            // Local control variables 

            HTuple hv_NumImages = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_DLSample = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_DLSampleBatch = new HTuple();
            try
            {
                //
                //This procedure creates DLSampleBatch, a tuple
                //containing a dictionary DLSample
                //for every image given in Images.
                //
                //Initialize output tuple.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images, out hv_NumImages);
                hv_DLSampleBatch.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLSampleBatch = HTuple.TupleGenConst(
                        hv_NumImages, -1);
                }
                //
                //Loop through all given images.
                HTuple end_val10 = hv_NumImages - 1;
                HTuple step_val10 = 1;
                for (hv_ImageIndex = 0; hv_ImageIndex.Continue(end_val10, step_val10); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val10))
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.SelectObj(ho_Images, out ho_Image, hv_ImageIndex + 1);
                    }
                    //Create DLSample from image.
                    hv_DLSample.Dispose();
                    HOperatorSet.CreateDict(out hv_DLSample);
                    HOperatorSet.SetDictObject(ho_Image, hv_DLSample, "image");
                    //
                    //Collect the DLSamples.
                    if (hv_DLSampleBatch == null)
                        hv_DLSampleBatch = new HTuple();
                    hv_DLSampleBatch[hv_ImageIndex] = hv_DLSample;
                }
                ho_Image.Dispose();

                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();

                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Generate a word to characters mapping. 
        private void gen_words_chars_mapping(HTuple hv_DLSample, out HTupleVector/*{eTupleVector,Dim=1}*/ hvec_WordsCharsMapping)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WordsIndices = new HTuple(), hv_CharsIndices = new HTuple();
            HTuple hv_WordLengths = new HTuple(), hv_WordArea = new HTuple();
            HTuple hv_CharArea = new HTuple(), hv_CharAreaThreshold = new HTuple();
            HTuple hv_WordIndex = new HTuple(), hv_AreaIntersection = new HTuple();
            HTuple hv_CIsInsideW = new HTuple(), hv_CIndex = new HTuple();
            // Initialize local and output iconic variables 
            hvec_WordsCharsMapping = new HTupleVector(1);
            try
            {
                //Procedure to generate the mapping: gen_words_chars_mapping
                if ((int)(new HTuple((new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WordsIndices.Dispose();
                        HOperatorSet.TupleFind(hv_DLSample.TupleGetDictTuple("bbox_label_id"), 0,
                            out hv_WordsIndices);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CharsIndices.Dispose();
                        HOperatorSet.TupleFind(hv_DLSample.TupleGetDictTuple("bbox_label_id"), 1,
                            out hv_CharsIndices);
                    }
                    if ((int)((new HTuple(hv_CharsIndices.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_WordsIndices.TupleNotEqual(
                        -1)))) != 0)
                    {
                        hv_WordLengths.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WordLengths = ((((hv_DLSample.TupleGetDictTuple(
                                "word"))).TupleSelect(hv_WordsIndices))).TupleStrlen();
                        }
                        //Init vector.
                        hvec_WordsCharsMapping[(new HTuple(((hv_DLSample.TupleGetDictTuple("bbox_label_id"))).TupleLength()
                            )) - 1] = new HTupleVector(new HTuple());
                        hv_WordArea.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_WordArea = (4 * (((hv_DLSample.TupleGetDictTuple(
                                "bbox_length1"))).TupleSelect(hv_WordsIndices))) * (((hv_DLSample.TupleGetDictTuple(
                                "bbox_length2"))).TupleSelect(hv_WordsIndices));
                        }
                        hv_CharArea.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CharArea = (4 * (((hv_DLSample.TupleGetDictTuple(
                                "bbox_length1"))).TupleSelect(hv_CharsIndices))) * (((hv_DLSample.TupleGetDictTuple(
                                "bbox_length2"))).TupleSelect(hv_CharsIndices));
                        }
                        //TODO: This threshold is quite arbitrary and not stable.
                        hv_CharAreaThreshold.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_CharAreaThreshold = hv_CharArea * 0.8;
                        }
                        for (hv_WordIndex = 0; (int)hv_WordIndex <= (int)((new HTuple(hv_WordsIndices.TupleLength()
                            )) - 1); hv_WordIndex = (int)hv_WordIndex + 1)
                        {
                            if ((int)(new HTuple(((hv_WordLengths.TupleSelect(hv_WordIndex))).TupleNotEqual(
                                0))) != 0)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_AreaIntersection.Dispose();
                                    HOperatorSet.AreaIntersectionRectangle2(((hv_DLSample.TupleGetDictTuple(
                                        "bbox_row"))).TupleSelect(hv_WordsIndices.TupleSelect(hv_WordIndex)),
                                        ((hv_DLSample.TupleGetDictTuple("bbox_col"))).TupleSelect(hv_WordsIndices.TupleSelect(
                                        hv_WordIndex)), ((hv_DLSample.TupleGetDictTuple("bbox_phi"))).TupleSelect(
                                        hv_WordsIndices.TupleSelect(hv_WordIndex)), ((hv_DLSample.TupleGetDictTuple(
                                        "bbox_length1"))).TupleSelect(hv_WordsIndices.TupleSelect(hv_WordIndex)),
                                        ((hv_DLSample.TupleGetDictTuple("bbox_length2"))).TupleSelect(hv_WordsIndices.TupleSelect(
                                        hv_WordIndex)), ((hv_DLSample.TupleGetDictTuple("bbox_row"))).TupleSelect(
                                        hv_CharsIndices), ((hv_DLSample.TupleGetDictTuple("bbox_col"))).TupleSelect(
                                        hv_CharsIndices), ((hv_DLSample.TupleGetDictTuple("bbox_phi"))).TupleSelect(
                                        hv_CharsIndices), ((hv_DLSample.TupleGetDictTuple("bbox_length1"))).TupleSelect(
                                        hv_CharsIndices), ((hv_DLSample.TupleGetDictTuple("bbox_length2"))).TupleSelect(
                                        hv_CharsIndices), out hv_AreaIntersection);
                                }
                                hv_CIsInsideW.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_CIsInsideW = hv_AreaIntersection.TupleGreaterElem(
                                        hv_CharAreaThreshold);
                                }
                                hv_CIndex.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_CIndex = hv_CIsInsideW.TupleFind(
                                        1);
                                }
                                if ((int)(new HTuple(hv_CIndex.TupleNotEqual(-1))) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        hvec_WordsCharsMapping[hv_WordsIndices.TupleSelect(
                                            hv_WordIndex)] = dh.Add(new HTupleVector(hv_CharsIndices.TupleSelect(
                                            hv_CIndex)));
                                    }
                                }
                            }
                            else
                            {
                                throw new HalconException(((("Sample with image id " + (hv_DLSample.TupleGetDictTuple(
                                    "image_id"))) + " is not valid. The word bounding box at index ") + hv_WordIndex) + " has an empty string as the ground truth. This is not allowed. Please assign a word label to every word bounding box.");
                            }
                        }
                    }
                }

                hv_WordsIndices.Dispose();
                hv_CharsIndices.Dispose();
                hv_WordLengths.Dispose();
                hv_WordArea.Dispose();
                hv_CharArea.Dispose();
                hv_CharAreaThreshold.Dispose();
                hv_WordIndex.Dispose();
                hv_AreaIntersection.Dispose();
                hv_CIsInsideW.Dispose();
                hv_CIndex.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WordsIndices.Dispose();
                hv_CharsIndices.Dispose();
                hv_WordLengths.Dispose();
                hv_WordArea.Dispose();
                hv_CharArea.Dispose();
                hv_CharAreaThreshold.Dispose();
                hv_WordIndex.Dispose();
                hv_AreaIntersection.Dispose();
                hv_CIsInsideW.Dispose();
                hv_CIndex.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess 3D data for deep-learning-based training and inference. 
        public void preprocess_dl_model_3d_data(HTuple hv_DLSample, HTuple hv_DLPreprocessParam)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageZ = null, ho_Domain, ho_Region = null;
            HObject ho_ImageReduced = null, ho_DomainComplement, ho_ImageX = null;
            HObject ho_ImageY = null, ho_ImageXYZ = null, ho_NXImage = null;
            HObject ho_NYImage = null, ho_NZImage = null, ho_MultiChannelImage;
            HObject ho___Tmp_Obj_0;

            // Local control variables 

            HTuple hv_HasNormals = new HTuple(), hv_XYZKeys = new HTuple();
            HTuple hv_HasXYZ = new HTuple(), hv_HasX = new HTuple();
            HTuple hv_HasY = new HTuple(), hv_HasZ = new HTuple();
            HTuple hv_HasFullXYZ = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_Type = new HTuple(), hv_Index = new HTuple();
            HTuple hv_Key = new HTuple(), hv_ZMinMaxExist = new HTuple();
            HTuple hv_GrayvalOutsideInit = new HTuple(), hv_NormalSizeExists = new HTuple();
            HTuple hv_NormalWidth = new HTuple(), hv_NormalHeight = new HTuple();
            HTuple hv_WidthZ = new HTuple(), hv_HeightZ = new HTuple();
            HTuple hv_ZoomNormals = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_ScaleWidth = new HTuple();
            HTuple hv_ScaleHeight = new HTuple(), hv_XIndex = new HTuple();
            HTuple hv_YIndex = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageZ);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_DomainComplement);
            HOperatorSet.GenEmptyObj(out ho_ImageX);
            HOperatorSet.GenEmptyObj(out ho_ImageY);
            HOperatorSet.GenEmptyObj(out ho_ImageXYZ);
            HOperatorSet.GenEmptyObj(out ho_NXImage);
            HOperatorSet.GenEmptyObj(out ho_NYImage);
            HOperatorSet.GenEmptyObj(out ho_NZImage);
            HOperatorSet.GenEmptyObj(out ho_MultiChannelImage);
            HOperatorSet.GenEmptyObj(out ho___Tmp_Obj_0);
            try
            {
                //
                //This procedure preprocesses 3D data of a DLSample.
                //
                //Check presence of inputs in DLSample.
                //
                hv_HasNormals.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "normals", out hv_HasNormals);
                hv_XYZKeys.Dispose();
                hv_XYZKeys = new HTuple();
                hv_XYZKeys[0] = "x";
                hv_XYZKeys[1] = "y";
                hv_XYZKeys[2] = "z";
                hv_HasXYZ.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", hv_XYZKeys, out hv_HasXYZ);
                hv_HasX.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HasX = hv_HasXYZ.TupleSelect(
                        0);
                }
                hv_HasY.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HasY = hv_HasXYZ.TupleSelect(
                        1);
                }
                hv_HasZ.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_HasZ = hv_HasXYZ.TupleSelect(
                        2);
                }
                hv_HasFullXYZ.Dispose();
                HOperatorSet.TupleMin(hv_HasXYZ, out hv_HasFullXYZ);
                if ((int)(hv_HasNormals.TupleNot()) != 0)
                {
                    //XYZ are required because normals would need to be computed.
                    if ((int)(hv_HasFullXYZ.TupleNot()) != 0)
                    {
                        throw new HalconException(new HTuple("The given input DLSample does not contain necessary images 'x','y' and 'z'. This is required if no normals are provided."));
                    }
                }
                else
                {
                    //At least Z is required if normals are given.
                    if ((int)(hv_HasZ.TupleNot()) != 0)
                    {
                        throw new HalconException(new HTuple("The given input DLSample does not contain at least the depth image 'z'. This is required because normals are provided. Optionally, 'x' and 'y' images might be provided additionally."));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(hv_DLSample.TupleGetDictObject("normals"), out hv_NumChannels);
                    }
                    if ((int)(new HTuple(hv_NumChannels.TupleNotEqual(3))) != 0)
                    {
                        throw new HalconException("The given input DLSample.normals has to have three channels.");
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Type.Dispose();
                        HOperatorSet.GetImageType(hv_DLSample.TupleGetDictObject("normals"), out hv_Type);
                    }
                    if ((int)(new HTuple(hv_Type.TupleNotEqual("real"))) != 0)
                    {
                        throw new HalconException("The given input DLSample.normals is not a real image.");
                    }
                }
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_HasXYZ.TupleLength())) - 1); hv_Index = (int)hv_Index + 1)
                {
                    if ((int)(hv_HasXYZ.TupleSelect(hv_Index)) != 0)
                    {
                        hv_Key.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Key = hv_XYZKeys.TupleSelect(
                                hv_Index);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_NumChannels.Dispose();
                            HOperatorSet.CountChannels(hv_DLSample.TupleGetDictObject(hv_Key), out hv_NumChannels);
                        }
                        if ((int)(new HTuple(hv_NumChannels.TupleNotEqual(1))) != 0)
                        {
                            throw new HalconException(("The given input DLSample." + hv_Key) + " needs to have a single channel.");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Type.Dispose();
                            HOperatorSet.GetImageType(hv_DLSample.TupleGetDictObject(hv_Key), out hv_Type);
                        }
                        if ((int)(new HTuple(hv_Type.TupleNotEqual("real"))) != 0)
                        {
                            throw new HalconException(("The given input DLSample." + hv_Key) + " is not a real image.");
                        }
                    }
                }
                //
                ho_ImageZ.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_ImageZ = hv_DLSample.TupleGetDictObject(
                        "z");
                }
                ho_Domain.Dispose();
                HOperatorSet.GetDomain(ho_ImageZ, out ho_Domain);
                //Reduce Z domain to user-defined min/max values for Z.
                hv_ZMinMaxExist.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", (new HTuple("min_z")).TupleConcat(
                    "max_z"), out hv_ZMinMaxExist);
                if ((int)(hv_ZMinMaxExist.TupleSelect(0)) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Region.Dispose();
                        HOperatorSet.Threshold(ho_ImageZ, out ho_Region, "min", hv_DLPreprocessParam.TupleGetDictTuple(
                            "min_z"));
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Difference(ho_Domain, ho_Region, out ExpTmpOutVar_0);
                        ho_Domain.Dispose();
                        ho_Domain = ExpTmpOutVar_0;
                    }
                }
                if ((int)(hv_ZMinMaxExist.TupleSelect(1)) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Region.Dispose();
                        HOperatorSet.Threshold(ho_ImageZ, out ho_Region, hv_DLPreprocessParam.TupleGetDictTuple(
                            "max_z"), "max");
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Difference(ho_Domain, ho_Region, out ExpTmpOutVar_0);
                        ho_Domain.Dispose();
                        ho_Domain = ExpTmpOutVar_0;
                    }
                }
                //Reduce domain because it might have changed
                if ((int)(hv_ZMinMaxExist.TupleMax()) != 0)
                {
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageZ, ho_Domain, out ho_ImageReduced);
                }
                ho_DomainComplement.Dispose();
                HOperatorSet.Complement(ho_Domain, out ho_DomainComplement);
                //
                //Before we zoom any 3D images we want to set all pixels outside of the domain to
                //an invalid value.
                hv_GrayvalOutsideInit.Dispose();
                hv_GrayvalOutsideInit = 0;

                if ((int)(hv_HasFullXYZ) != 0)
                {
                    ho_ImageX.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImageX = hv_DLSample.TupleGetDictObject(
                            "x");
                    }
                    ho_ImageY.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImageY = hv_DLSample.TupleGetDictObject(
                            "y");
                    }
                    ho_ImageZ.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImageZ = hv_DLSample.TupleGetDictObject(
                            "z");
                    }

                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_ImageX, out ExpTmpOutVar_0);
                        ho_ImageX.Dispose();
                        ho_ImageX = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_ImageY, out ExpTmpOutVar_0);
                        ho_ImageY.Dispose();
                        ho_ImageY = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_ImageZ, out ExpTmpOutVar_0);
                        ho_ImageZ.Dispose();
                        ho_ImageZ = ExpTmpOutVar_0;
                    }

                    HOperatorSet.OverpaintRegion(ho_ImageX, ho_DomainComplement, hv_GrayvalOutsideInit,
                        "fill");
                    HOperatorSet.OverpaintRegion(ho_ImageY, ho_DomainComplement, hv_GrayvalOutsideInit,
                        "fill");
                    HOperatorSet.OverpaintRegion(ho_ImageZ, ho_DomainComplement, hv_GrayvalOutsideInit,
                        "fill");

                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_ImageX, ho_Domain, out ExpTmpOutVar_0);
                        ho_ImageX.Dispose();
                        ho_ImageX = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_ImageY, ho_Domain, out ExpTmpOutVar_0);
                        ho_ImageY.Dispose();
                        ho_ImageY = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_ImageZ, ho_Domain, out ExpTmpOutVar_0);
                        ho_ImageZ.Dispose();
                        ho_ImageZ = ExpTmpOutVar_0;
                    }

                    if ((int)(hv_HasNormals.TupleNot()) != 0)
                    {
                        //Get optional user-defined resolution of normal computation.
                        hv_NormalSizeExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", (new HTuple("normal_image_width")).TupleConcat(
                            "normal_image_height"), out hv_NormalSizeExists);
                        if ((int)(((hv_NormalSizeExists.TupleSelect(0))).TupleNot()) != 0)
                        {
                            hv_NormalWidth.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NormalWidth = (((hv_DLPreprocessParam.TupleGetDictTuple(
                                    "image_width")) * 1.5)).TupleInt();
                            }
                        }
                        else
                        {
                            hv_NormalWidth.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NormalWidth = hv_DLPreprocessParam.TupleGetDictTuple(
                                    "normal_image_width");
                            }
                        }
                        if ((int)(((hv_NormalSizeExists.TupleSelect(1))).TupleNot()) != 0)
                        {
                            hv_NormalHeight.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NormalHeight = (((hv_DLPreprocessParam.TupleGetDictTuple(
                                    "image_height")) * 1.5)).TupleInt();
                            }
                        }
                        else
                        {
                            hv_NormalHeight.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NormalHeight = hv_DLPreprocessParam.TupleGetDictTuple(
                                    "normal_image_height");
                            }
                        }

                        hv_WidthZ.Dispose(); hv_HeightZ.Dispose();
                        HOperatorSet.GetImageSize(ho_ImageZ, out hv_WidthZ, out hv_HeightZ);
                        hv_ZoomNormals.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ZoomNormals = (new HTuple(hv_NormalWidth.TupleNotEqual(
                                hv_WidthZ))).TupleOr(new HTuple(hv_NormalHeight.TupleNotEqual(hv_HeightZ)));
                        }

                        if ((int)(hv_ZoomNormals) != 0)
                        {
                            ho_ImageXYZ.Dispose();
                            HOperatorSet.Compose3(ho_ImageX, ho_ImageY, ho_ImageZ, out ho_ImageXYZ
                                );
                            hv_Width.Dispose(); hv_Height.Dispose();
                            HOperatorSet.GetImageSize(ho_ImageXYZ, out hv_Width, out hv_Height);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ZoomImageSize(ho_ImageXYZ, out ExpTmpOutVar_0, hv_NormalWidth,
                                    hv_NormalHeight, "nearest_neighbor");
                                ho_ImageXYZ.Dispose();
                                ho_ImageXYZ = ExpTmpOutVar_0;
                            }
                            ho_ImageX.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageXYZ, out ho_ImageX, 1);
                            ho_ImageY.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageXYZ, out ho_ImageY, 2);
                            ho_ImageZ.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageXYZ, out ho_ImageZ, 3);
                            hv_ScaleWidth.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ScaleWidth = hv_NormalWidth / (hv_Width.TupleReal()
                                    );
                            }
                            hv_ScaleHeight.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ScaleHeight = hv_NormalHeight / (hv_Height.TupleReal()
                                    );
                            }
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ZoomRegion(ho_Domain, out ExpTmpOutVar_0, hv_ScaleWidth,
                                    hv_ScaleHeight);
                                ho_Domain.Dispose();
                                ho_Domain = ExpTmpOutVar_0;
                            }
                            {
                                HObject ExpTmpOutVar_0;
                                remove_invalid_3d_pixels(ho_ImageX, ho_ImageY, ho_ImageZ, ho_Domain,
                                    out ExpTmpOutVar_0, hv_GrayvalOutsideInit);
                                ho_Domain.Dispose();
                                ho_Domain = ExpTmpOutVar_0;
                            }
                            ho_DomainComplement.Dispose();
                            HOperatorSet.Complement(ho_Domain, out ho_DomainComplement);
                        }

                        ho_NXImage.Dispose(); ho_NYImage.Dispose(); ho_NZImage.Dispose();
                        compute_normals_xyz(ho_ImageX, ho_ImageY, ho_ImageZ, out ho_NXImage, out ho_NYImage,
                            out ho_NZImage, 1);
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_NXImage.Dispose();
                            HOperatorSet.AccessChannel(hv_DLSample.TupleGetDictObject("normals"), out ho_NXImage,
                                1);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_NYImage.Dispose();
                            HOperatorSet.AccessChannel(hv_DLSample.TupleGetDictObject("normals"), out ho_NYImage,
                                2);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_NZImage.Dispose();
                            HOperatorSet.AccessChannel(hv_DLSample.TupleGetDictObject("normals"), out ho_NZImage,
                                3);
                        }
                    }
                }
                else
                {
                    ho_ImageX.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImageX);
                    ho_ImageY.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImageY);

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_NXImage.Dispose();
                        HOperatorSet.AccessChannel(hv_DLSample.TupleGetDictObject("normals"), out ho_NXImage,
                            1);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_NYImage.Dispose();
                        HOperatorSet.AccessChannel(hv_DLSample.TupleGetDictObject("normals"), out ho_NYImage,
                            2);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_NZImage.Dispose();
                        HOperatorSet.AccessChannel(hv_DLSample.TupleGetDictObject("normals"), out ho_NZImage,
                            3);
                    }
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FullDomain(ho_ImageZ, out ExpTmpOutVar_0);
                    ho_ImageZ.Dispose();
                    ho_ImageZ = ExpTmpOutVar_0;
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FullDomain(ho_NXImage, out ExpTmpOutVar_0);
                    ho_NXImage.Dispose();
                    ho_NXImage = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FullDomain(ho_NYImage, out ExpTmpOutVar_0);
                    ho_NYImage.Dispose();
                    ho_NYImage = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.FullDomain(ho_NZImage, out ExpTmpOutVar_0);
                    ho_NZImage.Dispose();
                    ho_NZImage = ExpTmpOutVar_0;
                }

                //full_domain does not change the pixels outside of the existing domain.
                //Hence we have to set a specific value
                HOperatorSet.OverpaintRegion(ho_NXImage, ho_DomainComplement, hv_GrayvalOutsideInit,
                    "fill");
                HOperatorSet.OverpaintRegion(ho_NYImage, ho_DomainComplement, hv_GrayvalOutsideInit,
                    "fill");
                HOperatorSet.OverpaintRegion(ho_NZImage, ho_DomainComplement, hv_GrayvalOutsideInit,
                    "fill");
                HOperatorSet.OverpaintRegion(ho_ImageZ, ho_DomainComplement, hv_GrayvalOutsideInit,
                    "fill");

                ho_MultiChannelImage.Dispose();
                HOperatorSet.Compose4(ho_NXImage, ho_NYImage, ho_NZImage, ho_ImageZ, out ho_MultiChannelImage
                    );

                hv_HasX.Dispose();
                HOperatorSet.CountObj(ho_ImageX, out hv_HasX);
                if ((int)(hv_HasX) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_ImageX, out ExpTmpOutVar_0);
                        ho_ImageX.Dispose();
                        ho_ImageX = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.AppendChannel(ho_MultiChannelImage, ho_ImageX, out ExpTmpOutVar_0
                            );
                        ho_MultiChannelImage.Dispose();
                        ho_MultiChannelImage = ExpTmpOutVar_0;
                    }
                    hv_XIndex.Dispose();
                    HOperatorSet.CountChannels(ho_MultiChannelImage, out hv_XIndex);
                }
                hv_HasY.Dispose();
                HOperatorSet.CountObj(ho_ImageY, out hv_HasY);
                if ((int)(hv_HasY) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_ImageY, out ExpTmpOutVar_0);
                        ho_ImageY.Dispose();
                        ho_ImageY = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.AppendChannel(ho_MultiChannelImage, ho_ImageY, out ExpTmpOutVar_0
                            );
                        ho_MultiChannelImage.Dispose();
                        ho_MultiChannelImage = ExpTmpOutVar_0;
                    }
                    hv_YIndex.Dispose();
                    HOperatorSet.CountChannels(ho_MultiChannelImage, out hv_YIndex);
                }
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_MultiChannelImage, out hv_Width, out hv_Height);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ZoomImageSize(ho_MultiChannelImage, out ExpTmpOutVar_0, hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_width"), hv_DLPreprocessParam.TupleGetDictTuple("image_height"),
                        "nearest_neighbor");
                    ho_MultiChannelImage.Dispose();
                    ho_MultiChannelImage = ExpTmpOutVar_0;
                }

                ho_NXImage.Dispose(); ho_NYImage.Dispose(); ho_NZImage.Dispose(); ho_ImageZ.Dispose();
                HOperatorSet.Decompose4(ho_MultiChannelImage, out ho_NXImage, out ho_NYImage,
                    out ho_NZImage, out ho_ImageZ);
                if ((int)(hv_HasX) != 0)
                {
                    ho_ImageX.Dispose();
                    HOperatorSet.AccessChannel(ho_MultiChannelImage, out ho_ImageX, hv_XIndex);
                }
                if ((int)(hv_HasY) != 0)
                {
                    ho_ImageY.Dispose();
                    HOperatorSet.AccessChannel(ho_MultiChannelImage, out ho_ImageY, hv_YIndex);
                }


                //Zoom the domain
                hv_ScaleWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleWidth = (hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_width")) / (hv_Width.TupleReal());
                }
                hv_ScaleHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ScaleHeight = (hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_height")) / (hv_Height.TupleReal());
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ZoomRegion(ho_Domain, out ExpTmpOutVar_0, hv_ScaleWidth, hv_ScaleHeight);
                    ho_Domain.Dispose();
                    ho_Domain = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    remove_invalid_3d_pixels(ho_NXImage, ho_NYImage, ho_NZImage, ho_Domain, out ExpTmpOutVar_0,
                        hv_GrayvalOutsideInit);
                    ho_Domain.Dispose();
                    ho_Domain = ExpTmpOutVar_0;
                }

                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageX, ho_Domain, out ExpTmpOutVar_0);
                    ho_ImageX.Dispose();
                    ho_ImageX = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageY, ho_Domain, out ExpTmpOutVar_0);
                    ho_ImageY.Dispose();
                    ho_ImageY = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageZ, ho_Domain, out ExpTmpOutVar_0);
                    ho_ImageZ.Dispose();
                    ho_ImageZ = ExpTmpOutVar_0;
                }
                ho___Tmp_Obj_0.Dispose();
                HOperatorSet.Compose3(ho_NXImage, ho_NYImage, ho_NZImage, out ho___Tmp_Obj_0
                    );
                HOperatorSet.SetDictObject(ho___Tmp_Obj_0, hv_DLSample, "normals");
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho___Tmp_Obj_0.Dispose();
                    HOperatorSet.ReduceDomain(hv_DLSample.TupleGetDictObject("normals"), ho_Domain,
                        out ho___Tmp_Obj_0);
                }
                HOperatorSet.SetDictObject(ho___Tmp_Obj_0, hv_DLSample, "normals");

                //Overwrite preprocessed 3D data
                if ((int)(hv_HasX) != 0)
                {
                    HOperatorSet.SetDictObject(ho_ImageX, hv_DLSample, "x");
                }
                if ((int)(hv_HasY) != 0)
                {
                    HOperatorSet.SetDictObject(ho_ImageY, hv_DLSample, "y");
                }
                if ((int)(hv_HasZ) != 0)
                {
                    HOperatorSet.SetDictObject(ho_ImageZ, hv_DLSample, "z");
                }

                ho_ImageZ.Dispose();
                ho_Domain.Dispose();
                ho_Region.Dispose();
                ho_ImageReduced.Dispose();
                ho_DomainComplement.Dispose();
                ho_ImageX.Dispose();
                ho_ImageY.Dispose();
                ho_ImageXYZ.Dispose();
                ho_NXImage.Dispose();
                ho_NYImage.Dispose();
                ho_NZImage.Dispose();
                ho_MultiChannelImage.Dispose();
                ho___Tmp_Obj_0.Dispose();

                hv_HasNormals.Dispose();
                hv_XYZKeys.Dispose();
                hv_HasXYZ.Dispose();
                hv_HasX.Dispose();
                hv_HasY.Dispose();
                hv_HasZ.Dispose();
                hv_HasFullXYZ.Dispose();
                hv_NumChannels.Dispose();
                hv_Type.Dispose();
                hv_Index.Dispose();
                hv_Key.Dispose();
                hv_ZMinMaxExist.Dispose();
                hv_GrayvalOutsideInit.Dispose();
                hv_NormalSizeExists.Dispose();
                hv_NormalWidth.Dispose();
                hv_NormalHeight.Dispose();
                hv_WidthZ.Dispose();
                hv_HeightZ.Dispose();
                hv_ZoomNormals.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_XIndex.Dispose();
                hv_YIndex.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageZ.Dispose();
                ho_Domain.Dispose();
                ho_Region.Dispose();
                ho_ImageReduced.Dispose();
                ho_DomainComplement.Dispose();
                ho_ImageX.Dispose();
                ho_ImageY.Dispose();
                ho_ImageXYZ.Dispose();
                ho_NXImage.Dispose();
                ho_NYImage.Dispose();
                ho_NZImage.Dispose();
                ho_MultiChannelImage.Dispose();
                ho___Tmp_Obj_0.Dispose();

                hv_HasNormals.Dispose();
                hv_XYZKeys.Dispose();
                hv_HasXYZ.Dispose();
                hv_HasX.Dispose();
                hv_HasY.Dispose();
                hv_HasZ.Dispose();
                hv_HasFullXYZ.Dispose();
                hv_NumChannels.Dispose();
                hv_Type.Dispose();
                hv_Index.Dispose();
                hv_Key.Dispose();
                hv_ZMinMaxExist.Dispose();
                hv_GrayvalOutsideInit.Dispose();
                hv_NormalSizeExists.Dispose();
                hv_NormalWidth.Dispose();
                hv_NormalHeight.Dispose();
                hv_WidthZ.Dispose();
                hv_HeightZ.Dispose();
                hv_ZoomNormals.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_XIndex.Dispose();
                hv_YIndex.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess anomaly images for evaluation and visualization of deep-learning-based anomaly detection or Global Context Anomaly Detection. 
        public void preprocess_dl_model_anomaly(HObject ho_AnomalyImages, out HObject ho_AnomalyImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            // Local copy input parameter variables 
            HObject ho_AnomalyImages_COPY_INP_TMP;
            ho_AnomalyImages_COPY_INP_TMP = new HObject(ho_AnomalyImages);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_ImageWidthInput = new HTuple(), hv_ImageHeightInput = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_NumMatches = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_EqualByte = new HTuple();
            HTuple hv_NumChannelsAllImages = new HTuple(), hv_ImageNumChannelsTuple = new HTuple();
            HTuple hv_IndicesWrongChannels = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagesPreprocessed);
            try
            {
                //
                //This procedure preprocesses the anomaly images given by AnomalyImages
                //according to the parameters in the dictionary DLPreprocessParam.
                //Note that depending on the images,
                //additional preprocessing steps might be beneficial.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                hv_ImageNumChannels.Dispose();
                hv_ImageNumChannels = 1;
                //
                //Preprocess the images.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleEqual("anomaly_detection")))) != 0)
                {
                    //The option 'keep_domain' is only supported for models of 'type' = 'anomaly_detection'
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                HOperatorSet.MinMaxGray(ho_AnomalyImages_COPY_INP_TMP, ho_AnomalyImages_COPY_INP_TMP,
                    0, out hv_Min, out hv_Max, out hv_Range);
                if ((int)(new HTuple(hv_Min.TupleLess(0.0))) != 0)
                {
                    throw new HalconException("Values of anomaly image must not be smaller than 0.0.");
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthInput.Dispose(); hv_ImageHeightInput.Dispose();
                HOperatorSet.GetImageSize(ho_AnomalyImages_COPY_INP_TMP, out hv_ImageWidthInput,
                    out hv_ImageHeightInput);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthInput);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightInput);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_AnomalyImages_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|real", out hv_NumMatches);
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte' or 'real'.");
                }
                //
                //If the type is 'byte', convert it to 'real' and scale it.
                //The gray value scaling does not work on 'byte' images.
                //For 'real' images it is assumed that the range is already correct.
                hv_EqualByte.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualByte = hv_Type.TupleEqualElem(
                        "byte");
                }
                if ((int)(new HTuple(((hv_EqualByte.TupleMax())).TupleEqual(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_EqualByte.TupleMin())).TupleEqual(0))) != 0)
                    {
                        throw new HalconException("Passing mixed type images is not supported.");
                    }
                    //Convert the image type from 'byte' to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the number of channels.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                //Check all images for number of channels.
                hv_NumChannelsAllImages.Dispose();
                HOperatorSet.CountChannels(ho_AnomalyImages_COPY_INP_TMP, out hv_NumChannelsAllImages);
                hv_ImageNumChannelsTuple.Dispose();
                HOperatorSet.TupleGenConst(hv_NumImages, hv_ImageNumChannels, out hv_ImageNumChannelsTuple);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IndicesWrongChannels.Dispose();
                    HOperatorSet.TupleFind(hv_NumChannelsAllImages.TupleNotEqualElem(hv_ImageNumChannelsTuple),
                        1, out hv_IndicesWrongChannels);
                }
                //
                //Check for anomaly image channels.
                //Only single channel images are accepted.
                if ((int)(new HTuple(hv_IndicesWrongChannels.TupleNotEqual(-1))) != 0)
                {
                    throw new HalconException("Number of channels in anomaly image is not supported. Please check for anomaly images with a number of channels different from 1.");
                }
                //
                //Write preprocessed image to output variable.
                ho_AnomalyImagesPreprocessed.Dispose();
                ho_AnomalyImagesPreprocessed = new HObject(ho_AnomalyImages_COPY_INP_TMP);
                //
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess the provided DLSample image for augmentation purposes. 
        public void preprocess_dl_model_augmentation_data(HTuple hv_DLSample, HTuple hv_DLPreprocessParam)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_InputImage = null, ho_ImageHighRes = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_AugmentationKeyExists = new HTuple(), hv_ImageKeyExists = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ImageType = new HTuple(), hv_InputImageWidth = new HTuple();
            HTuple hv_InputImageHeight = new HTuple(), hv_InputImageWidthHeightRatio = new HTuple();
            HTuple hv_ZoomHeight = new HTuple(), hv_ZoomWidth = new HTuple();
            HTuple hv_HasPadding = new HTuple(), hv_ZoomFactorWidth = new HTuple();
            HTuple hv_ZoomFactorHeight = new HTuple(), hv_UseZoomImage = new HTuple();
            HTuple hv_DLSampleHighRes = new HTuple(), hv_DLPreprocessParamHighRes = new HTuple();
            HTuple hv___Tmp_Ctrl_Dict_Init_0 = new HTuple(), hv___Tmp_Ctrl_Dict_Init_1 = new HTuple();
            HTuple hv___Tmp_Ctrl_Dict_Init_2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_InputImage);
            HOperatorSet.GenEmptyObj(out ho_ImageHighRes);
            try
            {
                //This procedure preprocesses the provided DLSample image for augmentation purposes.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the required preprocessing parameters.
                hv_ImageWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageWidth = hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_width");
                }
                hv_ImageHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageHeight = hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_height");
                }
                hv_ImageNumChannels.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageNumChannels = hv_DLPreprocessParam.TupleGetDictTuple(
                        "image_num_channels");
                }
                hv_ModelType.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ModelType = hv_DLPreprocessParam.TupleGetDictTuple(
                        "model_type");
                }
                //
                //Determine whether the preprocessing is required or not.
                hv_AugmentationKeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "augmentation",
                    out hv_AugmentationKeyExists);
                if ((int)(hv_AugmentationKeyExists.TupleNot()) != 0)
                {
                    ho_InputImage.Dispose();
                    ho_ImageHighRes.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ModelType.Dispose();
                    hv_AugmentationKeyExists.Dispose();
                    hv_ImageKeyExists.Dispose();
                    hv_NumImages.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ImageType.Dispose();
                    hv_InputImageWidth.Dispose();
                    hv_InputImageHeight.Dispose();
                    hv_InputImageWidthHeightRatio.Dispose();
                    hv_ZoomHeight.Dispose();
                    hv_ZoomWidth.Dispose();
                    hv_HasPadding.Dispose();
                    hv_ZoomFactorWidth.Dispose();
                    hv_ZoomFactorHeight.Dispose();
                    hv_UseZoomImage.Dispose();
                    hv_DLSampleHighRes.Dispose();
                    hv_DLPreprocessParamHighRes.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_2.Dispose();

                    return;
                }
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_0);
                HOperatorSet.SetDictTuple(hv___Tmp_Ctrl_Dict_Init_0, "comp", "true");
                if ((int)(((((hv_DLPreprocessParam.TupleConcat(hv___Tmp_Ctrl_Dict_Init_0))).TupleTestEqualDictItem(
                    "augmentation", "comp"))).TupleNot()) != 0)
                {
                    ho_InputImage.Dispose();
                    ho_ImageHighRes.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ModelType.Dispose();
                    hv_AugmentationKeyExists.Dispose();
                    hv_ImageKeyExists.Dispose();
                    hv_NumImages.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ImageType.Dispose();
                    hv_InputImageWidth.Dispose();
                    hv_InputImageHeight.Dispose();
                    hv_InputImageWidthHeightRatio.Dispose();
                    hv_ZoomHeight.Dispose();
                    hv_ZoomWidth.Dispose();
                    hv_HasPadding.Dispose();
                    hv_ZoomFactorWidth.Dispose();
                    hv_ZoomFactorHeight.Dispose();
                    hv_UseZoomImage.Dispose();
                    hv_DLSampleHighRes.Dispose();
                    hv_DLPreprocessParamHighRes.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_2.Dispose();

                    return;
                }
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv___Tmp_Ctrl_Dict_Init_0 = HTuple.TupleConstant(
                        "HNULL");
                }
                if ((int)((new HTuple(hv_ModelType.TupleNotEqual("ocr_detection"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleNotEqual("ocr_recognition")))) != 0)
                {
                    ho_InputImage.Dispose();
                    ho_ImageHighRes.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ModelType.Dispose();
                    hv_AugmentationKeyExists.Dispose();
                    hv_ImageKeyExists.Dispose();
                    hv_NumImages.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ImageType.Dispose();
                    hv_InputImageWidth.Dispose();
                    hv_InputImageHeight.Dispose();
                    hv_InputImageWidthHeightRatio.Dispose();
                    hv_ZoomHeight.Dispose();
                    hv_ZoomWidth.Dispose();
                    hv_HasPadding.Dispose();
                    hv_ZoomFactorWidth.Dispose();
                    hv_ZoomFactorHeight.Dispose();
                    hv_UseZoomImage.Dispose();
                    hv_DLSampleHighRes.Dispose();
                    hv_DLPreprocessParamHighRes.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                    hv___Tmp_Ctrl_Dict_Init_2.Dispose();

                    return;
                }
                //
                //Get the input image and its properties.
                hv_ImageKeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "image", out hv_ImageKeyExists);
                if ((int)(hv_ImageKeyExists.TupleNot()) != 0)
                {
                    throw new HalconException("The sample to process needs to include an image.");
                }
                ho_InputImage.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_InputImage = hv_DLSample.TupleGetDictObject(
                        "image");
                }
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_InputImage, out hv_NumImages);
                if ((int)(new HTuple(hv_NumImages.TupleNotEqual(1))) != 0)
                {
                    throw new HalconException("The sample to process needs to include exactly 1 image.");
                }
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_InputImage, out hv_NumChannels);
                hv_ImageType.Dispose();
                HOperatorSet.GetImageType(ho_InputImage, out hv_ImageType);
                hv_InputImageWidth.Dispose(); hv_InputImageHeight.Dispose();
                HOperatorSet.GetImageSize(ho_InputImage, out hv_InputImageWidth, out hv_InputImageHeight);
                //
                //Execute model specific preprocessing.
                if ((int)(new HTuple(hv_ModelType.TupleEqual("ocr_recognition"))) != 0)
                {
                    if ((int)(new HTuple(hv_ImageNumChannels.TupleNotEqual(1))) != 0)
                    {
                        throw new HalconException("The only 'image_num_channels' value supported for ocr_recognition models is 1.");
                    }
                    if ((int)(new HTuple((new HTuple(hv_ImageType.TupleRegexpTest("byte|real"))).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Please provide only images of type 'byte' or 'real' for ocr_recognition models.");
                    }
                    if ((int)(new HTuple((new HTuple((new HTuple(((hv_NumChannels.TupleEqualElem(
                        1))).TupleOr(hv_NumChannels.TupleEqualElem(3)))).TupleSum())).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Please provide only 1- or 3-channels images for ocr_recognition models.");
                    }
                    //
                    ho_ImageHighRes.Dispose();
                    HOperatorSet.FullDomain(ho_InputImage, out ho_ImageHighRes);
                    if ((int)(new HTuple(hv_NumChannels.TupleEqual(3))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Rgb1ToGray(ho_ImageHighRes, out ExpTmpOutVar_0);
                            ho_ImageHighRes.Dispose();
                            ho_ImageHighRes = ExpTmpOutVar_0;
                        }
                    }
                    hv_InputImageWidthHeightRatio.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageWidthHeightRatio = hv_InputImageWidth / (hv_InputImageHeight.TupleReal()
                            );
                    }
                    hv_ZoomHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomHeight = hv_InputImageHeight.TupleMin2(
                            2 * hv_ImageHeight);
                    }
                    hv_ZoomWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomWidth = ((hv_ZoomHeight * hv_InputImageWidthHeightRatio)).TupleInt()
                            ;
                    }
                    hv_HasPadding.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HasPadding = new HTuple(((((hv_ImageHeight * hv_InputImageWidthHeightRatio)).TupleInt()
                            )).TupleLess(hv_ImageWidth));
                    }
                    if ((int)((new HTuple(hv_ZoomHeight.TupleGreater(hv_ImageHeight))).TupleOr(
                        hv_HasPadding)) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ZoomImageSize(ho_ImageHighRes, out ExpTmpOutVar_0, hv_ZoomWidth,
                                hv_ZoomHeight, "constant");
                            ho_ImageHighRes.Dispose();
                            ho_ImageHighRes = ExpTmpOutVar_0;
                        }
                        hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                        HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_1);
                        HOperatorSet.SetDictTuple(hv_DLSample, "augmentation_data", hv___Tmp_Ctrl_Dict_Init_1);
                        hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv___Tmp_Ctrl_Dict_Init_1 = HTuple.TupleConstant(
                                "HNULL");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictObject(ho_ImageHighRes, hv_DLSample.TupleGetDictTuple(
                                "augmentation_data"), "image_high_res");
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_DLSample.TupleGetDictTuple("augmentation_data"),
                                "preprocess_params", hv_DLPreprocessParam);
                        }
                    }
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("ocr_detection"))) != 0)
                {
                    if ((int)(new HTuple(hv_ImageNumChannels.TupleNotEqual(3))) != 0)
                    {
                        throw new HalconException("The only 'image_num_channels' value supported for ocr_detection models is 3.");
                    }
                    if ((int)(new HTuple((new HTuple(hv_ImageType.TupleRegexpTest("byte|real"))).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Please provide only images of type 'byte' or 'real' for ocr_detection models.");
                    }
                    if ((int)(new HTuple((new HTuple((new HTuple(((hv_NumChannels.TupleEqualElem(
                        1))).TupleOr(hv_NumChannels.TupleEqualElem(3)))).TupleSum())).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Please provide only 1- or 3-channels images for ocr_detection models.");
                    }
                    //
                    //Calculate aspect-ratio preserving zoom dimensions for high resolution.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomFactorWidth.Dispose(); hv_ZoomFactorHeight.Dispose();
                        calculate_dl_image_zoom_factors(hv_InputImageWidth, hv_InputImageHeight,
                            2 * hv_ImageWidth, 2 * hv_ImageHeight, hv_DLPreprocessParam, out hv_ZoomFactorWidth,
                            out hv_ZoomFactorHeight);
                    }
                    hv_ZoomHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomHeight = ((hv_ZoomFactorHeight * hv_InputImageHeight)).TupleRound()
                            ;
                    }
                    hv_ZoomWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomWidth = ((hv_ZoomFactorWidth * hv_InputImageWidth)).TupleRound()
                            ;
                    }
                    //
                    //Use the better size for high resolution: 2x resolution size of preprocess image or input image size.
                    hv_UseZoomImage.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_UseZoomImage = (new HTuple(hv_ZoomWidth.TupleLess(
                            hv_InputImageWidth))).TupleOr(new HTuple(hv_ZoomHeight.TupleLess(hv_InputImageHeight)));
                    }
                    hv_DLSampleHighRes.Dispose();
                    HOperatorSet.CopyDict(hv_DLSample, new HTuple(), new HTuple(), out hv_DLSampleHighRes);
                    hv_DLPreprocessParamHighRes.Dispose();
                    HOperatorSet.CopyDict(hv_DLPreprocessParam, new HTuple(), new HTuple(), out hv_DLPreprocessParamHighRes);
                    //
                    ho_ImageHighRes.Dispose();
                    HOperatorSet.FullDomain(ho_InputImage, out ho_ImageHighRes);
                    if ((int)(hv_UseZoomImage) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParamHighRes, "image_width", hv_ZoomWidth);
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParamHighRes, "image_height",
                            hv_ZoomHeight);
                        preprocess_dl_model_bbox_rect2(ho_ImageHighRes, hv_DLSampleHighRes, hv_DLPreprocessParamHighRes);
                        gen_dl_ocr_detection_targets(hv_DLSampleHighRes, hv_DLPreprocessParamHighRes);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ZoomImageSize(ho_ImageHighRes, out ExpTmpOutVar_0, hv_ZoomWidth,
                                hv_ZoomHeight, "constant");
                            ho_ImageHighRes.Dispose();
                            ho_ImageHighRes = ExpTmpOutVar_0;
                        }
                    }
                    else
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParamHighRes, "image_width", hv_InputImageWidth);
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParamHighRes, "image_height",
                            hv_InputImageHeight);
                        gen_dl_ocr_detection_targets(hv_DLSampleHighRes, hv_DLPreprocessParamHighRes);
                    }
                    HOperatorSet.SetDictObject(ho_ImageHighRes, hv_DLSampleHighRes, "image");
                    //
                    hv___Tmp_Ctrl_Dict_Init_2.Dispose();
                    HOperatorSet.CreateDict(out hv___Tmp_Ctrl_Dict_Init_2);
                    HOperatorSet.SetDictTuple(hv_DLSample, "augmentation_data", hv___Tmp_Ctrl_Dict_Init_2);
                    hv___Tmp_Ctrl_Dict_Init_2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv___Tmp_Ctrl_Dict_Init_2 = HTuple.TupleConstant(
                            "HNULL");
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_DLSample.TupleGetDictTuple("augmentation_data"),
                            "sample_high_res", hv_DLSampleHighRes);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.SetDictTuple(hv_DLSample.TupleGetDictTuple("augmentation_data"),
                            "preprocess_params", hv_DLPreprocessParam);
                    }
                }
                //
                ho_InputImage.Dispose();
                ho_ImageHighRes.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ModelType.Dispose();
                hv_AugmentationKeyExists.Dispose();
                hv_ImageKeyExists.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageType.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_InputImageWidthHeightRatio.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_HasPadding.Dispose();
                hv_ZoomFactorWidth.Dispose();
                hv_ZoomFactorHeight.Dispose();
                hv_UseZoomImage.Dispose();
                hv_DLSampleHighRes.Dispose();
                hv_DLPreprocessParamHighRes.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                hv___Tmp_Ctrl_Dict_Init_2.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_InputImage.Dispose();
                ho_ImageHighRes.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ModelType.Dispose();
                hv_AugmentationKeyExists.Dispose();
                hv_ImageKeyExists.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageType.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_InputImageWidthHeightRatio.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_HasPadding.Dispose();
                hv_ZoomFactorWidth.Dispose();
                hv_ZoomFactorHeight.Dispose();
                hv_UseZoomImage.Dispose();
                hv_DLSampleHighRes.Dispose();
                hv_DLPreprocessParamHighRes.Dispose();
                hv___Tmp_Ctrl_Dict_Init_0.Dispose();
                hv___Tmp_Ctrl_Dict_Init_1.Dispose();
                hv___Tmp_Ctrl_Dict_Init_2.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection and Instance Segmentation
        // Short Description: Preprocess the bounding boxes of type 'rectangle1' for a given sample. 
        private void preprocess_dl_model_bbox_rect1(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho_DomainRaw = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_BBoxCol1 = new HTuple();
            HTuple hv_BBoxCol2 = new HTuple(), hv_BBoxRow1 = new HTuple();
            HTuple hv_BBoxRow2 = new HTuple(), hv_BBoxLabel = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_ImageId = new HTuple();
            HTuple hv_ExceptionMessage = new HTuple(), hv_BoxesInvalid = new HTuple();
            HTuple hv_DomainRow1 = new HTuple(), hv_DomainColumn1 = new HTuple();
            HTuple hv_DomainRow2 = new HTuple(), hv_DomainColumn2 = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_MaskDelete = new HTuple(), hv_MaskNewBbox = new HTuple();
            HTuple hv_BBoxCol1New = new HTuple(), hv_BBoxCol2New = new HTuple();
            HTuple hv_BBoxRow1New = new HTuple(), hv_BBoxRow2New = new HTuple();
            HTuple hv_BBoxLabelNew = new HTuple(), hv_FactorResampleWidth = new HTuple();
            HTuple hv_FactorResampleHeight = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainRaw);
            try
            {
                //
                //This procedure preprocesses the bounding boxes of type 'rectangle1' for a given sample.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //
                //Get bounding box coordinates and labels.
                try
                {
                    hv_BBoxCol1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col1", out hv_BBoxCol1);
                    hv_BBoxCol2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col2", out hv_BBoxCol2);
                    hv_BBoxRow1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row1", out hv_BBoxRow1);
                    hv_BBoxRow2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row2", out hv_BBoxRow2);
                    hv_BBoxLabel.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BBoxLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ImageId.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1302))) != 0)
                    {
                        hv_ExceptionMessage.Dispose();
                        hv_ExceptionMessage = "A bounding box coordinate key is missing.";
                    }
                    else
                    {
                        hv_ExceptionMessage.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ExceptionMessage = hv_Exception.TupleSelect(
                                2);
                        }
                    }
                    throw new HalconException((("An error has occurred during preprocessing image_id " + hv_ImageId) + " when getting bounding box coordinates : ") + hv_ExceptionMessage);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow1.TupleLength())).TupleGreater(0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = ((hv_BBoxRow1.TupleGreaterEqualElem(
                            hv_BBoxRow2))).TupleOr(hv_BBoxCol1.TupleGreaterEqualElem(hv_BBoxCol2));
                    }
                    if ((int)(new HTuple(((hv_BoxesInvalid.TupleSum())).TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one box with zero-area, i.e. bbox_col1 >= bbox_col2 or bbox_row1 >= bbox_row2."));
                    }
                }
                else
                {
                    //There are no bounding boxes, hence nothing to do.
                    ho_DomainRaw.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_BBoxCol1.Dispose();
                    hv_BBoxCol2.Dispose();
                    hv_BBoxRow1.Dispose();
                    hv_BBoxRow2.Dispose();
                    hv_BBoxLabel.Dispose();
                    hv_Exception.Dispose();
                    hv_ImageId.Dispose();
                    hv_ExceptionMessage.Dispose();
                    hv_BoxesInvalid.Dispose();
                    hv_DomainRow1.Dispose();
                    hv_DomainColumn1.Dispose();
                    hv_DomainRow2.Dispose();
                    hv_DomainColumn2.Dispose();
                    hv_WidthRaw.Dispose();
                    hv_HeightRaw.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                    hv_Row2.Dispose();
                    hv_Col2.Dispose();
                    hv_MaskDelete.Dispose();
                    hv_MaskNewBbox.Dispose();
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxLabelNew.Dispose();
                    hv_FactorResampleWidth.Dispose();
                    hv_FactorResampleHeight.Dispose();

                    return;
                }
                //
                //If the domain is cropped, crop bounding boxes.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //
                    //Get domain.
                    ho_DomainRaw.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_DomainRaw);
                    //
                    //Set the size of the raw image to the domain extensions.
                    hv_DomainRow1.Dispose(); hv_DomainColumn1.Dispose(); hv_DomainRow2.Dispose(); hv_DomainColumn2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_DomainRaw, out hv_DomainRow1, out hv_DomainColumn1,
                        out hv_DomainRow2, out hv_DomainColumn2);
                    //The domain is always given as a pixel-precise region.
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_DomainColumn2 - hv_DomainColumn1) + 1.0;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_DomainRow2 - hv_DomainRow1) + 1.0;
                    }
                    //
                    //Crop the bounding boxes.
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = hv_BBoxRow1.TupleMax2(
                            hv_DomainRow1 - .5);
                    }
                    hv_Col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col1 = hv_BBoxCol1.TupleMax2(
                            hv_DomainColumn1 - .5);
                    }
                    hv_Row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row2 = hv_BBoxRow2.TupleMin2(
                            hv_DomainRow2 + .5);
                    }
                    hv_Col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col2 = hv_BBoxCol2.TupleMin2(
                            hv_DomainColumn2 + .5);
                    }
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = ((hv_Row1.TupleGreaterEqualElem(
                            hv_Row2))).TupleOr(hv_Col1.TupleGreaterEqualElem(hv_Col2));
                    }
                    hv_MaskNewBbox.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskNewBbox = 1 - hv_MaskDelete;
                    }
                    //Store the preprocessed bounding box entries.
                    hv_BBoxCol1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxCol1New = (hv_Col1.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainColumn1;
                    }
                    hv_BBoxCol2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxCol2New = (hv_Col2.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainColumn1;
                    }
                    hv_BBoxRow1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRow1New = (hv_Row1.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainRow1;
                    }
                    hv_BBoxRow2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRow2New = (hv_Row2.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainRow1;
                    }
                    hv_BBoxLabelNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLabelNew = hv_BBoxLabel.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    //
                    //If we remove/select bounding boxes we also need to filter the corresponding
                    //instance segmentation masks if they exist.
                    filter_dl_sample_instance_segmentation_masks(hv_DLSample, hv_MaskNewBbox);
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    //If the entire image is used, set the variables accordingly.
                    //Get the original size.
                    hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                    //Set new coordinates to input coordinates.
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol1New = new HTuple(hv_BBoxCol1);
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxCol2New = new HTuple(hv_BBoxCol2);
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow1New = new HTuple(hv_BBoxRow1);
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxRow2New = new HTuple(hv_BBoxRow2);
                    hv_BBoxLabelNew.Dispose();
                    hv_BBoxLabelNew = new HTuple(hv_BBoxLabel);
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Rescale the bounding boxes.
                //
                //Get required images width and height.
                //
                //Only rescale bounding boxes if the required image dimensions are not the raw dimensions.
                if ((int)((new HTuple(hv_ImageHeight.TupleNotEqual(hv_HeightRaw))).TupleOr(
                    new HTuple(hv_ImageWidth.TupleNotEqual(hv_WidthRaw)))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }
                    //Rescale the bounding box coordinates.
                    //As we use XLD-coordinates we temporarily move the boxes by (.5,.5) for rescaling.
                    //Doing so, the center of the XLD-coordinate system (-0.5,-0.5) is used
                    //for scaling, hence the scaling is performed w.r.t. the pixel coordinate system.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol1New = ((hv_BBoxCol1New + .5) * hv_FactorResampleWidth) - .5;
                            hv_BBoxCol1New.Dispose();
                            hv_BBoxCol1New = ExpTmpLocalVar_BBoxCol1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol2New = ((hv_BBoxCol2New + .5) * hv_FactorResampleWidth) - .5;
                            hv_BBoxCol2New.Dispose();
                            hv_BBoxCol2New = ExpTmpLocalVar_BBoxCol2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow1New = ((hv_BBoxRow1New + .5) * hv_FactorResampleHeight) - .5;
                            hv_BBoxRow1New.Dispose();
                            hv_BBoxRow1New = ExpTmpLocalVar_BBoxRow1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow2New = ((hv_BBoxRow2New + .5) * hv_FactorResampleHeight) - .5;
                            hv_BBoxRow2New.Dispose();
                            hv_BBoxRow2New = ExpTmpLocalVar_BBoxRow2New;
                        }
                    }
                    //
                }
                //
                //Make a final check and remove bounding boxes that have zero area.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow1New.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = ((hv_BBoxRow1New.TupleGreaterEqualElem(
                            hv_BBoxRow2New))).TupleOr(hv_BBoxCol1New.TupleGreaterEqualElem(hv_BBoxCol2New));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol1New = hv_BBoxCol1New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxCol1New.Dispose();
                            hv_BBoxCol1New = ExpTmpLocalVar_BBoxCol1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol2New = hv_BBoxCol2New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxCol2New.Dispose();
                            hv_BBoxCol2New = ExpTmpLocalVar_BBoxCol2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow1New = hv_BBoxRow1New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxRow1New.Dispose();
                            hv_BBoxRow1New = ExpTmpLocalVar_BBoxRow1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow2New = hv_BBoxRow2New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxRow2New.Dispose();
                            hv_BBoxRow2New = ExpTmpLocalVar_BBoxRow2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxLabelNew = hv_BBoxLabelNew.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxLabelNew.Dispose();
                            hv_BBoxLabelNew = ExpTmpLocalVar_BBoxLabelNew;
                        }
                    }
                    //
                    //If we remove/select bounding boxes we also need to filter the corresponding
                    //instance segmentation masks if they exist.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        filter_dl_sample_instance_segmentation_masks(hv_DLSample, 1 - hv_MaskDelete);
                    }
                }
                //
                //Set new bounding box coordinates in the dictionary.
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col1", hv_BBoxCol1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col2", hv_BBoxCol2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row1", hv_BBoxRow1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row2", hv_BBoxRow2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BBoxLabelNew);
                //
                ho_DomainRaw.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DomainRaw.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection and Instance Segmentation
        // Short Description: Preprocess the bounding boxes of type 'rectangle2' for a given sample. 
        private void preprocess_dl_model_bbox_rect2(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho_DomainRaw = null, ho_Rectangle2XLD = null;
            HObject ho_Rectangle2XLDSheared = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_BBoxRow = new HTuple(), hv_BBoxCol = new HTuple();
            HTuple hv_BBoxLength1 = new HTuple(), hv_BBoxLength2 = new HTuple();
            HTuple hv_BBoxPhi = new HTuple(), hv_BBoxLabel = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_ImageId = new HTuple();
            HTuple hv_ExceptionMessage = new HTuple(), hv_BoxesInvalid = new HTuple();
            HTuple hv_DomainRow1 = new HTuple(), hv_DomainColumn1 = new HTuple();
            HTuple hv_DomainRow2 = new HTuple(), hv_DomainColumn2 = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_MaskDelete = new HTuple(), hv_MaskNewBbox = new HTuple();
            HTuple hv_BBoxRowNew = new HTuple(), hv_BBoxColNew = new HTuple();
            HTuple hv_BBoxLength1New = new HTuple(), hv_BBoxLength2New = new HTuple();
            HTuple hv_BBoxPhiNew = new HTuple(), hv_BBoxLabelNew = new HTuple();
            HTuple hv_ClassIDsNoOrientationIndices = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ClassIDsNoOrientationIndicesTmp = new HTuple();
            HTuple hv_DirectionLength1Row = new HTuple(), hv_DirectionLength1Col = new HTuple();
            HTuple hv_DirectionLength2Row = new HTuple(), hv_DirectionLength2Col = new HTuple();
            HTuple hv_Corner1Row = new HTuple(), hv_Corner1Col = new HTuple();
            HTuple hv_Corner2Row = new HTuple(), hv_Corner2Col = new HTuple();
            HTuple hv_FactorResampleWidth = new HTuple(), hv_FactorResampleHeight = new HTuple();
            HTuple hv_BBoxRow1 = new HTuple(), hv_BBoxCol1 = new HTuple();
            HTuple hv_BBoxRow2 = new HTuple(), hv_BBoxCol2 = new HTuple();
            HTuple hv_BBoxRow3 = new HTuple(), hv_BBoxCol3 = new HTuple();
            HTuple hv_BBoxRow4 = new HTuple(), hv_BBoxCol4 = new HTuple();
            HTuple hv_BBoxCol1New = new HTuple(), hv_BBoxCol2New = new HTuple();
            HTuple hv_BBoxCol3New = new HTuple(), hv_BBoxCol4New = new HTuple();
            HTuple hv_BBoxRow1New = new HTuple(), hv_BBoxRow2New = new HTuple();
            HTuple hv_BBoxRow3New = new HTuple(), hv_BBoxRow4New = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple(), hv_HomMat2DScale = new HTuple();
            HTuple hv__ = new HTuple(), hv_BBoxPhiTmp = new HTuple();
            HTuple hv_PhiDelta = new HTuple(), hv_PhiDeltaNegativeIndices = new HTuple();
            HTuple hv_IndicesRot90 = new HTuple(), hv_IndicesRot180 = new HTuple();
            HTuple hv_IndicesRot270 = new HTuple(), hv_SwapIndices = new HTuple();
            HTuple hv_Tmp = new HTuple(), hv_BBoxPhiNewIndices = new HTuple();
            HTuple hv_PhiThreshold = new HTuple(), hv_PhiToCorrect = new HTuple();
            HTuple hv_NumCorrections = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainRaw);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2XLD);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2XLDSheared);
            try
            {
                //This procedure preprocesses the bounding boxes of type 'rectangle2' for a given sample.
                //
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get preprocess parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //The keys 'ignore_direction' and 'class_ids_no_orientation' are optional.
                hv_IgnoreDirection.Dispose();
                hv_IgnoreDirection = 0;
                hv_ClassIDsNoOrientation.Dispose();
                hv_ClassIDsNoOrientation = new HTuple();
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", (new HTuple("ignore_direction")).TupleConcat(
                    "class_ids_no_orientation"), out hv_KeyExists);
                if ((int)(hv_KeyExists.TupleSelect(0)) != 0)
                {
                    hv_IgnoreDirection.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_direction", out hv_IgnoreDirection);
                    if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("true"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        hv_IgnoreDirection = 1;
                    }
                    else if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("false"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        hv_IgnoreDirection = 0;
                    }
                }
                if ((int)(hv_KeyExists.TupleSelect(1)) != 0)
                {
                    hv_ClassIDsNoOrientation.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_no_orientation",
                        out hv_ClassIDsNoOrientation);
                }
                //
                //Get bounding box coordinates and labels.
                try
                {
                    hv_BBoxRow.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row", out hv_BBoxRow);
                    hv_BBoxCol.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col", out hv_BBoxCol);
                    hv_BBoxLength1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length1", out hv_BBoxLength1);
                    hv_BBoxLength2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length2", out hv_BBoxLength2);
                    hv_BBoxPhi.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_phi", out hv_BBoxPhi);
                    hv_BBoxLabel.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BBoxLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ImageId.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1302))) != 0)
                    {
                        hv_ExceptionMessage.Dispose();
                        hv_ExceptionMessage = "A bounding box coordinate key is missing.";
                    }
                    else
                    {
                        hv_ExceptionMessage.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ExceptionMessage = hv_Exception.TupleSelect(
                                2);
                        }
                    }
                    throw new HalconException((("An error has occurred during preprocessing image_id " + hv_ImageId) + " when getting bounding box coordinates : ") + hv_ExceptionMessage);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow.TupleLength())).TupleGreater(0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = (((hv_BBoxLength1.TupleEqualElem(
                            0))).TupleSum()) + (((hv_BBoxLength2.TupleEqualElem(0))).TupleSum());
                    }
                    if ((int)(new HTuple(hv_BoxesInvalid.TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one bounding box with zero-area, i.e. bbox_length1 == 0 or bbox_length2 == 0!"));
                    }
                }
                else
                {
                    //There are no bounding boxes, hence nothing to do.
                    ho_DomainRaw.Dispose();
                    ho_Rectangle2XLD.Dispose();
                    ho_Rectangle2XLDSheared.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_IgnoreDirection.Dispose();
                    hv_ClassIDsNoOrientation.Dispose();
                    hv_KeyExists.Dispose();
                    hv_BBoxRow.Dispose();
                    hv_BBoxCol.Dispose();
                    hv_BBoxLength1.Dispose();
                    hv_BBoxLength2.Dispose();
                    hv_BBoxPhi.Dispose();
                    hv_BBoxLabel.Dispose();
                    hv_Exception.Dispose();
                    hv_ImageId.Dispose();
                    hv_ExceptionMessage.Dispose();
                    hv_BoxesInvalid.Dispose();
                    hv_DomainRow1.Dispose();
                    hv_DomainColumn1.Dispose();
                    hv_DomainRow2.Dispose();
                    hv_DomainColumn2.Dispose();
                    hv_WidthRaw.Dispose();
                    hv_HeightRaw.Dispose();
                    hv_MaskDelete.Dispose();
                    hv_MaskNewBbox.Dispose();
                    hv_BBoxRowNew.Dispose();
                    hv_BBoxColNew.Dispose();
                    hv_BBoxLength1New.Dispose();
                    hv_BBoxLength2New.Dispose();
                    hv_BBoxPhiNew.Dispose();
                    hv_BBoxLabelNew.Dispose();
                    hv_ClassIDsNoOrientationIndices.Dispose();
                    hv_Index.Dispose();
                    hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                    hv_DirectionLength1Row.Dispose();
                    hv_DirectionLength1Col.Dispose();
                    hv_DirectionLength2Row.Dispose();
                    hv_DirectionLength2Col.Dispose();
                    hv_Corner1Row.Dispose();
                    hv_Corner1Col.Dispose();
                    hv_Corner2Row.Dispose();
                    hv_Corner2Col.Dispose();
                    hv_FactorResampleWidth.Dispose();
                    hv_FactorResampleHeight.Dispose();
                    hv_BBoxRow1.Dispose();
                    hv_BBoxCol1.Dispose();
                    hv_BBoxRow2.Dispose();
                    hv_BBoxCol2.Dispose();
                    hv_BBoxRow3.Dispose();
                    hv_BBoxCol3.Dispose();
                    hv_BBoxRow4.Dispose();
                    hv_BBoxCol4.Dispose();
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxCol3New.Dispose();
                    hv_BBoxCol4New.Dispose();
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxRow3New.Dispose();
                    hv_BBoxRow4New.Dispose();
                    hv_HomMat2DIdentity.Dispose();
                    hv_HomMat2DScale.Dispose();
                    hv__.Dispose();
                    hv_BBoxPhiTmp.Dispose();
                    hv_PhiDelta.Dispose();
                    hv_PhiDeltaNegativeIndices.Dispose();
                    hv_IndicesRot90.Dispose();
                    hv_IndicesRot180.Dispose();
                    hv_IndicesRot270.Dispose();
                    hv_SwapIndices.Dispose();
                    hv_Tmp.Dispose();
                    hv_BBoxPhiNewIndices.Dispose();
                    hv_PhiThreshold.Dispose();
                    hv_PhiToCorrect.Dispose();
                    hv_NumCorrections.Dispose();

                    return;
                }
                //
                //If the domain is cropped, crop bounding boxes.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //
                    //Get domain.
                    ho_DomainRaw.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_DomainRaw);
                    //
                    //Set the size of the raw image to the domain extensions.
                    hv_DomainRow1.Dispose(); hv_DomainColumn1.Dispose(); hv_DomainRow2.Dispose(); hv_DomainColumn2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_DomainRaw, out hv_DomainRow1, out hv_DomainColumn1,
                        out hv_DomainRow2, out hv_DomainColumn2);
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_DomainColumn2 - hv_DomainColumn1) + 1;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_DomainRow2 - hv_DomainRow1) + 1;
                    }
                    //
                    //Crop the bounding boxes.
                    //Remove the boxes with center outside of the domain.
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = (new HTuple((new HTuple(((hv_BBoxRow.TupleLessElem(
                            hv_DomainRow1))).TupleOr(hv_BBoxCol.TupleLessElem(hv_DomainColumn1)))).TupleOr(
                            hv_BBoxRow.TupleGreaterElem(hv_DomainRow2)))).TupleOr(hv_BBoxCol.TupleGreaterElem(
                            hv_DomainColumn2));
                    }
                    hv_MaskNewBbox.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskNewBbox = 1 - hv_MaskDelete;
                    }
                    //Store the preprocessed bounding box entries.
                    hv_BBoxRowNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRowNew = (hv_BBoxRow.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainRow1;
                    }
                    hv_BBoxColNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxColNew = (hv_BBoxCol.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainColumn1;
                    }
                    hv_BBoxLength1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLength1New = hv_BBoxLength1.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxLength2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLength2New = hv_BBoxLength2.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxPhiNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxPhiNew = hv_BBoxPhi.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxLabelNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLabelNew = hv_BBoxLabel.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    //
                    //If we remove/select bounding boxes we also need to filter the corresponding
                    //instance segmentation masks if they exist.
                    filter_dl_sample_instance_segmentation_masks(hv_DLSample, hv_MaskNewBbox);
                    //
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    //If the entire image is used, set the variables accordingly.
                    //Get the original size.
                    hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                    //Set new coordinates to input coordinates.
                    hv_BBoxRowNew.Dispose();
                    hv_BBoxRowNew = new HTuple(hv_BBoxRow);
                    hv_BBoxColNew.Dispose();
                    hv_BBoxColNew = new HTuple(hv_BBoxCol);
                    hv_BBoxLength1New.Dispose();
                    hv_BBoxLength1New = new HTuple(hv_BBoxLength1);
                    hv_BBoxLength2New.Dispose();
                    hv_BBoxLength2New = new HTuple(hv_BBoxLength2);
                    hv_BBoxPhiNew.Dispose();
                    hv_BBoxPhiNew = new HTuple(hv_BBoxPhi);
                    hv_BBoxLabelNew.Dispose();
                    hv_BBoxLabelNew = new HTuple(hv_BBoxLabel);
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Generate smallest enclosing axis-aligned bounding box for classes in ClassIDsNoOrientation.
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_ClassIDsNoOrientationIndices = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ClassIDsNoOrientationIndicesTmp = ((hv_BBoxLabelNew.TupleEqualElem(
                            hv_ClassIDsNoOrientation.TupleSelect(hv_Index)))).TupleFind(1);
                    }
                    if ((int)(new HTuple(hv_ClassIDsNoOrientationIndicesTmp.TupleNotEqual(-1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ClassIDsNoOrientationIndices = hv_ClassIDsNoOrientationIndices.TupleConcat(
                                    hv_ClassIDsNoOrientationIndicesTmp);
                                hv_ClassIDsNoOrientationIndices.Dispose();
                                hv_ClassIDsNoOrientationIndices = ExpTmpLocalVar_ClassIDsNoOrientationIndices;
                            }
                        }
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_ClassIDsNoOrientationIndices.TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Calculate length1 and length2 using position of corners.
                    hv_DirectionLength1Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength1Row = -(((hv_BBoxPhiNew.TupleSelect(
                            hv_ClassIDsNoOrientationIndices))).TupleSin());
                    }
                    hv_DirectionLength1Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength1Col = ((hv_BBoxPhiNew.TupleSelect(
                            hv_ClassIDsNoOrientationIndices))).TupleCos();
                    }
                    hv_DirectionLength2Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength2Row = -hv_DirectionLength1Col;
                    }
                    hv_DirectionLength2Col.Dispose();
                    hv_DirectionLength2Col = new HTuple(hv_DirectionLength1Row);
                    hv_Corner1Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner1Row = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Row) + ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Row);
                    }
                    hv_Corner1Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner1Col = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Col) + ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Col);
                    }
                    hv_Corner2Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner2Row = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Row) - ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Row);
                    }
                    hv_Corner2Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner2Col = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Col) - ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Col);
                    }
                    //
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_ClassIDsNoOrientationIndices] = 0.0;
                    if (hv_BBoxLength1New == null)
                        hv_BBoxLength1New = new HTuple();
                    hv_BBoxLength1New[hv_ClassIDsNoOrientationIndices] = ((hv_Corner1Col.TupleAbs()
                        )).TupleMax2(hv_Corner2Col.TupleAbs());
                    if (hv_BBoxLength2New == null)
                        hv_BBoxLength2New = new HTuple();
                    hv_BBoxLength2New[hv_ClassIDsNoOrientationIndices] = ((hv_Corner1Row.TupleAbs()
                        )).TupleMax2(hv_Corner2Row.TupleAbs());
                }
                //
                //Rescale bounding boxes.
                //
                //Get required images width and height.
                //
                //Only rescale bounding boxes if the required image dimensions are not the raw dimensions.
                if ((int)((new HTuple(hv_ImageHeight.TupleNotEqual(hv_HeightRaw))).TupleOr(
                    new HTuple(hv_ImageWidth.TupleNotEqual(hv_WidthRaw)))) != 0)
                {
                    //
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose(); hv_FactorResampleHeight.Dispose();
                    calculate_dl_image_zoom_factors(hv_WidthRaw, hv_HeightRaw, hv_ImageWidth,
                        hv_ImageHeight, hv_DLPreprocessParam, out hv_FactorResampleWidth, out hv_FactorResampleHeight);
                    //
                    if ((int)((new HTuple(hv_FactorResampleHeight.TupleNotEqual(hv_FactorResampleWidth))).TupleAnd(
                        new HTuple((new HTuple(hv_BBoxRowNew.TupleLength())).TupleGreater(0)))) != 0)
                    {
                        //In order to preserve the correct orientation we have to transform the points individually.
                        //Get the coordinates of the four corner points.
                        hv_BBoxRow1.Dispose(); hv_BBoxCol1.Dispose(); hv_BBoxRow2.Dispose(); hv_BBoxCol2.Dispose(); hv_BBoxRow3.Dispose(); hv_BBoxCol3.Dispose(); hv_BBoxRow4.Dispose(); hv_BBoxCol4.Dispose();
                        convert_rect2_5to8param(hv_BBoxRowNew, hv_BBoxColNew, hv_BBoxLength1New,
                            hv_BBoxLength2New, hv_BBoxPhiNew, out hv_BBoxRow1, out hv_BBoxCol1,
                            out hv_BBoxRow2, out hv_BBoxCol2, out hv_BBoxRow3, out hv_BBoxCol3,
                            out hv_BBoxRow4, out hv_BBoxCol4);
                        //
                        //Rescale the coordinates.
                        hv_BBoxCol1New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol1New = hv_BBoxCol1 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol2New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol2New = hv_BBoxCol2 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol3New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol3New = hv_BBoxCol3 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol4New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol4New = hv_BBoxCol4 * hv_FactorResampleWidth;
                        }
                        hv_BBoxRow1New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow1New = hv_BBoxRow1 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow2New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow2New = hv_BBoxRow2 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow3New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow3New = hv_BBoxRow3 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow4New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow4New = hv_BBoxRow4 * hv_FactorResampleHeight;
                        }
                        //
                        //The rectangles will get sheared, that is why new rectangles have to be found.
                        //Generate homography to scale rectangles.
                        hv_HomMat2DIdentity.Dispose();
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        hv_HomMat2DScale.Dispose();
                        HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, hv_FactorResampleHeight,
                            hv_FactorResampleWidth, 0, 0, out hv_HomMat2DScale);
                        //Generate XLD contours for the rectangles.
                        ho_Rectangle2XLD.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle2XLD, hv_BBoxRowNew,
                            hv_BBoxColNew, hv_BBoxPhiNew, hv_BBoxLength1New, hv_BBoxLength2New);
                        //Scale the XLD contours --> results in sheared regions.
                        ho_Rectangle2XLDSheared.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_Rectangle2XLD, out ho_Rectangle2XLDSheared,
                            hv_HomMat2DScale);
                        hv_BBoxRowNew.Dispose(); hv_BBoxColNew.Dispose(); hv_BBoxPhiNew.Dispose(); hv_BBoxLength1New.Dispose(); hv_BBoxLength2New.Dispose();
                        HOperatorSet.SmallestRectangle2Xld(ho_Rectangle2XLDSheared, out hv_BBoxRowNew,
                            out hv_BBoxColNew, out hv_BBoxPhiNew, out hv_BBoxLength1New, out hv_BBoxLength2New);
                        //
                        //smallest_rectangle2_xld might change the orientation of the bounding box.
                        //Hence, take the orientation that is closest to the one obtained out of the 4 corner points.
                        hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv_BBoxPhiTmp.Dispose();
                        convert_rect2_8to5param(hv_BBoxRow1New, hv_BBoxCol1New, hv_BBoxRow2New,
                            hv_BBoxCol2New, hv_BBoxRow3New, hv_BBoxCol3New, hv_BBoxRow4New, hv_BBoxCol4New,
                            hv_IgnoreDirection, out hv__, out hv__, out hv__, out hv__, out hv_BBoxPhiTmp);
                        hv_PhiDelta.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PhiDelta = ((hv_BBoxPhiTmp - hv_BBoxPhiNew)).TupleFmod(
                                (new HTuple(360)).TupleRad());
                        }
                        //Guarantee that angles are positive.
                        hv_PhiDeltaNegativeIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PhiDeltaNegativeIndices = ((hv_PhiDelta.TupleLessElem(
                                0.0))).TupleFind(1);
                        }
                        if ((int)(new HTuple(hv_PhiDeltaNegativeIndices.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_PhiDelta == null)
                                hv_PhiDelta = new HTuple();
                            hv_PhiDelta[hv_PhiDeltaNegativeIndices] = (hv_PhiDelta.TupleSelect(hv_PhiDeltaNegativeIndices)) + ((new HTuple(360)).TupleRad()
                                );
                        }
                        hv_IndicesRot90.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot90 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(45)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(135)).TupleRad())))).TupleFind(1);
                        }
                        hv_IndicesRot180.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot180 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(135)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(225)).TupleRad())))).TupleFind(1);
                        }
                        hv_IndicesRot270.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot270 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(225)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(315)).TupleRad())))).TupleFind(1);
                        }
                        hv_SwapIndices.Dispose();
                        hv_SwapIndices = new HTuple();
                        if ((int)(new HTuple(hv_IndicesRot90.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot90] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot90)) + ((new HTuple(90)).TupleRad()
                                );
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SwapIndices = hv_SwapIndices.TupleConcat(
                                        hv_IndicesRot90);
                                    hv_SwapIndices.Dispose();
                                    hv_SwapIndices = ExpTmpLocalVar_SwapIndices;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_IndicesRot180.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot180] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot180)) + ((new HTuple(180)).TupleRad()
                                );
                        }
                        if ((int)(new HTuple(hv_IndicesRot270.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot270] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot270)) + ((new HTuple(270)).TupleRad()
                                );
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SwapIndices = hv_SwapIndices.TupleConcat(
                                        hv_IndicesRot270);
                                    hv_SwapIndices.Dispose();
                                    hv_SwapIndices = ExpTmpLocalVar_SwapIndices;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_SwapIndices.TupleNotEqual(new HTuple()))) != 0)
                        {
                            hv_Tmp.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Tmp = hv_BBoxLength1New.TupleSelect(
                                    hv_SwapIndices);
                            }
                            if (hv_BBoxLength1New == null)
                                hv_BBoxLength1New = new HTuple();
                            hv_BBoxLength1New[hv_SwapIndices] = hv_BBoxLength2New.TupleSelect(hv_SwapIndices);
                            if (hv_BBoxLength2New == null)
                                hv_BBoxLength2New = new HTuple();
                            hv_BBoxLength2New[hv_SwapIndices] = hv_Tmp;
                        }
                        //Change angles such that they lie in the range (-180°, 180°].
                        hv_BBoxPhiNewIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxPhiNewIndices = ((hv_BBoxPhiNew.TupleGreaterElem(
                                (new HTuple(180)).TupleRad()))).TupleFind(1);
                        }
                        if ((int)(new HTuple(hv_BBoxPhiNewIndices.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_BBoxPhiNewIndices] = (hv_BBoxPhiNew.TupleSelect(hv_BBoxPhiNewIndices)) - ((new HTuple(360)).TupleRad()
                                );
                        }
                        //
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxColNew = hv_BBoxColNew * hv_FactorResampleWidth;
                                hv_BBoxColNew.Dispose();
                                hv_BBoxColNew = ExpTmpLocalVar_BBoxColNew;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxRowNew = hv_BBoxRowNew * hv_FactorResampleWidth;
                                hv_BBoxRowNew.Dispose();
                                hv_BBoxRowNew = ExpTmpLocalVar_BBoxRowNew;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxLength1New = hv_BBoxLength1New * hv_FactorResampleWidth;
                                hv_BBoxLength1New.Dispose();
                                hv_BBoxLength1New = ExpTmpLocalVar_BBoxLength1New;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxLength2New = hv_BBoxLength2New * hv_FactorResampleWidth;
                                hv_BBoxLength2New.Dispose();
                                hv_BBoxLength2New = ExpTmpLocalVar_BBoxLength2New;
                            }
                        }
                        //Phi stays the same.
                    }
                    //
                }
                //
                //Adapt the bounding box angles such that they are within the correct range,
                //which is (-180°,180°] for 'ignore_direction'==false and (-90°,90°] else.
                hv_PhiThreshold.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiThreshold = ((new HTuple(180)).TupleRad()
                        ) - (hv_IgnoreDirection * ((new HTuple(90)).TupleRad()));
                }
                hv_PhiDelta.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiDelta = 2 * hv_PhiThreshold;
                }
                //Correct angles that are too large.
                hv_PhiToCorrect.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiToCorrect = ((hv_BBoxPhiNew.TupleGreaterElem(
                        hv_PhiThreshold))).TupleFind(1);
                }
                if ((int)((new HTuple(hv_PhiToCorrect.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_PhiToCorrect.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    hv_NumCorrections.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumCorrections = (((((hv_BBoxPhiNew.TupleSelect(
                            hv_PhiToCorrect)) - hv_PhiThreshold) / hv_PhiDelta)).TupleInt()) + 1;
                    }
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_PhiToCorrect] = (hv_BBoxPhiNew.TupleSelect(hv_PhiToCorrect)) - (hv_NumCorrections * hv_PhiDelta);
                }
                //Correct angles that are too small.
                hv_PhiToCorrect.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiToCorrect = ((hv_BBoxPhiNew.TupleLessEqualElem(
                        -hv_PhiThreshold))).TupleFind(1);
                }
                if ((int)((new HTuple(hv_PhiToCorrect.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_PhiToCorrect.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    hv_NumCorrections.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumCorrections = (((((((hv_BBoxPhiNew.TupleSelect(
                            hv_PhiToCorrect)) + hv_PhiThreshold)).TupleAbs()) / hv_PhiDelta)).TupleInt()
                            ) + 1;
                    }
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_PhiToCorrect] = (hv_BBoxPhiNew.TupleSelect(hv_PhiToCorrect)) + (hv_NumCorrections * hv_PhiDelta);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRowNew.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = (((hv_BBoxLength1New.TupleEqualElem(
                            0))).TupleSum()) + (((hv_BBoxLength2New.TupleEqualElem(0))).TupleSum());
                    }
                    if ((int)(new HTuple(hv_BoxesInvalid.TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one box with zero-area, i.e. bbox_length1 == 0 or bbox_length2 == 0!"));
                    }
                }
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row", hv_BBoxRowNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col", hv_BBoxColNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length1", hv_BBoxLength1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length2", hv_BBoxLength2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_phi", hv_BBoxPhiNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BBoxLabelNew);
                //
                ho_DomainRaw.Dispose();
                ho_Rectangle2XLD.Dispose();
                ho_Rectangle2XLDSheared.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_KeyExists.Dispose();
                hv_BBoxRow.Dispose();
                hv_BBoxCol.Dispose();
                hv_BBoxLength1.Dispose();
                hv_BBoxLength2.Dispose();
                hv_BBoxPhi.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxRowNew.Dispose();
                hv_BBoxColNew.Dispose();
                hv_BBoxLength1New.Dispose();
                hv_BBoxLength2New.Dispose();
                hv_BBoxPhiNew.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_Index.Dispose();
                hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                hv_DirectionLength1Row.Dispose();
                hv_DirectionLength1Col.Dispose();
                hv_DirectionLength2Row.Dispose();
                hv_DirectionLength2Col.Dispose();
                hv_Corner1Row.Dispose();
                hv_Corner1Col.Dispose();
                hv_Corner2Row.Dispose();
                hv_Corner2Col.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow3.Dispose();
                hv_BBoxCol3.Dispose();
                hv_BBoxRow4.Dispose();
                hv_BBoxCol4.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxCol3New.Dispose();
                hv_BBoxCol4New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxRow3New.Dispose();
                hv_BBoxRow4New.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv__.Dispose();
                hv_BBoxPhiTmp.Dispose();
                hv_PhiDelta.Dispose();
                hv_PhiDeltaNegativeIndices.Dispose();
                hv_IndicesRot90.Dispose();
                hv_IndicesRot180.Dispose();
                hv_IndicesRot270.Dispose();
                hv_SwapIndices.Dispose();
                hv_Tmp.Dispose();
                hv_BBoxPhiNewIndices.Dispose();
                hv_PhiThreshold.Dispose();
                hv_PhiToCorrect.Dispose();
                hv_NumCorrections.Dispose();

                return;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DomainRaw.Dispose();
                ho_Rectangle2XLD.Dispose();
                ho_Rectangle2XLDSheared.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_KeyExists.Dispose();
                hv_BBoxRow.Dispose();
                hv_BBoxCol.Dispose();
                hv_BBoxLength1.Dispose();
                hv_BBoxLength2.Dispose();
                hv_BBoxPhi.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxRowNew.Dispose();
                hv_BBoxColNew.Dispose();
                hv_BBoxLength1New.Dispose();
                hv_BBoxLength2New.Dispose();
                hv_BBoxPhiNew.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_Index.Dispose();
                hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                hv_DirectionLength1Row.Dispose();
                hv_DirectionLength1Col.Dispose();
                hv_DirectionLength2Row.Dispose();
                hv_DirectionLength2Col.Dispose();
                hv_Corner1Row.Dispose();
                hv_Corner1Col.Dispose();
                hv_Corner2Row.Dispose();
                hv_Corner2Col.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow3.Dispose();
                hv_BBoxCol3.Dispose();
                hv_BBoxRow4.Dispose();
                hv_BBoxCol4.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxCol3New.Dispose();
                hv_BBoxCol4New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxRow3New.Dispose();
                hv_BBoxRow4New.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv__.Dispose();
                hv_BBoxPhiTmp.Dispose();
                hv_PhiDelta.Dispose();
                hv_PhiDeltaNegativeIndices.Dispose();
                hv_IndicesRot90.Dispose();
                hv_IndicesRot180.Dispose();
                hv_IndicesRot270.Dispose();
                hv_SwapIndices.Dispose();
                hv_Tmp.Dispose();
                hv_BBoxPhiNewIndices.Dispose();
                hv_PhiThreshold.Dispose();
                hv_PhiToCorrect.Dispose();
                hv_NumCorrections.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess images for deep-learning-based training and inference. 
        public void preprocess_dl_model_images(HObject ho_Images, out HObject ho_ImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_PreservedDomains = null, ho_ImageSelected = null;
            HObject ho_DomainSelected = null, ho_ImagesScaled = null, ho_ImageScaled = null;
            HObject ho_Channel = null, ho_ChannelScaled = null, ho_ThreeChannelImage = null;
            HObject ho_SingleChannelImage = null;

            // Local copy input parameter variables 
            HObject ho_Images_COPY_INP_TMP;
            ho_Images_COPY_INP_TMP = new HObject(ho_Images);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ImageRangeMin = new HTuple();
            HTuple hv_ImageRangeMax = new HTuple(), hv_DomainHandling = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_Type = new HTuple();
            HTuple hv_NumMatches = new HTuple(), hv_InputNumChannels = new HTuple();
            HTuple hv_OutputNumChannels = new HTuple(), hv_NumChannels1 = new HTuple();
            HTuple hv_NumChannels3 = new HTuple(), hv_AreInputNumChannels1 = new HTuple();
            HTuple hv_AreInputNumChannels3 = new HTuple(), hv_AreInputNumChannels1Or3 = new HTuple();
            HTuple hv_ValidNumChannels = new HTuple(), hv_ValidNumChannelsText = new HTuple();
            HTuple hv_PreserveDomain = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_UniqRow1 = new HTuple();
            HTuple hv_UniqColumn1 = new HTuple(), hv_UniqRow2 = new HTuple();
            HTuple hv_UniqColumn2 = new HTuple(), hv_RectangleIndex = new HTuple();
            HTuple hv_OriginalWidth = new HTuple(), hv_OriginalHeight = new HTuple();
            HTuple hv_UniqWidth = new HTuple(), hv_UniqHeight = new HTuple();
            HTuple hv_ScaleWidth = new HTuple(), hv_ScaleHeight = new HTuple();
            HTuple hv_ScaleIndex = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_NumChannels = new HTuple(), hv_ChannelIndex = new HTuple();
            HTuple hv_Min = new HTuple(), hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Shift = new HTuple();
            HTuple hv_MeanValues = new HTuple(), hv_DeviationValues = new HTuple();
            HTuple hv_UseDefaultNormalizationValues = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_RescaleRange = new HTuple(), hv_CurrentNumChannels = new HTuple();
            HTuple hv_DiffNumChannelsIndices = new HTuple(), hv_Index = new HTuple();
            HTuple hv_DiffNumChannelsIndex = new HTuple(), hv_NumDomains = new HTuple();
            HTuple hv_DomainIndex = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagesPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_PreservedDomains);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_DomainSelected);
            HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Channel);
            HOperatorSet.GenEmptyObj(out ho_ChannelScaled);
            HOperatorSet.GenEmptyObj(out ho_ThreeChannelImage);
            HOperatorSet.GenEmptyObj(out ho_SingleChannelImage);
            try
            {
                //
                //This procedure preprocesses the provided Images according to the parameters in
                //the dictionary DLPreprocessParam. Note that depending on the images, additional
                //preprocessing steps might be beneficial.
                //
                //Validate the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_NormalizationType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Validate the type of the input images.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumImages.TupleEqual(0))) != 0)
                {
                    throw new HalconException("Please provide some images to preprocess.");
                }
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|int|real", out hv_NumMatches);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException(new HTuple("Please provide only images of type 'byte', 'int1', 'int2', 'uint2', 'int4', 'int8', or 'real'."));
                }
                //
                //Handle ocr_recognition models.
                if ((int)(new HTuple(hv_ModelType.TupleEqual("ocr_recognition"))) != 0)
                {
                    ho_ImagesPreprocessed.Dispose();
                    preprocess_dl_model_images_ocr_recognition(ho_Images_COPY_INP_TMP, out ho_ImagesPreprocessed,
                        hv_DLPreprocessParam);
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_PreservedDomains.Dispose();
                    ho_ImageSelected.Dispose();
                    ho_DomainSelected.Dispose();
                    ho_ImagesScaled.Dispose();
                    ho_ImageScaled.Dispose();
                    ho_Channel.Dispose();
                    ho_ChannelScaled.Dispose();
                    ho_ThreeChannelImage.Dispose();
                    ho_SingleChannelImage.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ImageRangeMin.Dispose();
                    hv_ImageRangeMax.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_NormalizationType.Dispose();
                    hv_ModelType.Dispose();
                    hv_NumImages.Dispose();
                    hv_Type.Dispose();
                    hv_NumMatches.Dispose();
                    hv_InputNumChannels.Dispose();
                    hv_OutputNumChannels.Dispose();
                    hv_NumChannels1.Dispose();
                    hv_NumChannels3.Dispose();
                    hv_AreInputNumChannels1.Dispose();
                    hv_AreInputNumChannels3.Dispose();
                    hv_AreInputNumChannels1Or3.Dispose();
                    hv_ValidNumChannels.Dispose();
                    hv_ValidNumChannelsText.Dispose();
                    hv_PreserveDomain.Dispose();
                    hv_Row1.Dispose();
                    hv_Column1.Dispose();
                    hv_Row2.Dispose();
                    hv_Column2.Dispose();
                    hv_UniqRow1.Dispose();
                    hv_UniqColumn1.Dispose();
                    hv_UniqRow2.Dispose();
                    hv_UniqColumn2.Dispose();
                    hv_RectangleIndex.Dispose();
                    hv_OriginalWidth.Dispose();
                    hv_OriginalHeight.Dispose();
                    hv_UniqWidth.Dispose();
                    hv_UniqHeight.Dispose();
                    hv_ScaleWidth.Dispose();
                    hv_ScaleHeight.Dispose();
                    hv_ScaleIndex.Dispose();
                    hv_ImageIndex.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ChannelIndex.Dispose();
                    hv_Min.Dispose();
                    hv_Max.Dispose();
                    hv_Range.Dispose();
                    hv_Scale.Dispose();
                    hv_Shift.Dispose();
                    hv_MeanValues.Dispose();
                    hv_DeviationValues.Dispose();
                    hv_UseDefaultNormalizationValues.Dispose();
                    hv_Exception.Dispose();
                    hv_Indices.Dispose();
                    hv_RescaleRange.Dispose();
                    hv_CurrentNumChannels.Dispose();
                    hv_DiffNumChannelsIndices.Dispose();
                    hv_Index.Dispose();
                    hv_DiffNumChannelsIndex.Dispose();
                    hv_NumDomains.Dispose();
                    hv_DomainIndex.Dispose();

                    return;
                }
                //
                //Handle ocr_detection models.
                if ((int)(new HTuple(hv_ModelType.TupleEqual("ocr_detection"))) != 0)
                {
                    ho_ImagesPreprocessed.Dispose();
                    preprocess_dl_model_images_ocr_detection(ho_Images_COPY_INP_TMP, out ho_ImagesPreprocessed,
                        hv_DLPreprocessParam);
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_PreservedDomains.Dispose();
                    ho_ImageSelected.Dispose();
                    ho_DomainSelected.Dispose();
                    ho_ImagesScaled.Dispose();
                    ho_ImageScaled.Dispose();
                    ho_Channel.Dispose();
                    ho_ChannelScaled.Dispose();
                    ho_ThreeChannelImage.Dispose();
                    ho_SingleChannelImage.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_ImageNumChannels.Dispose();
                    hv_ImageRangeMin.Dispose();
                    hv_ImageRangeMax.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_NormalizationType.Dispose();
                    hv_ModelType.Dispose();
                    hv_NumImages.Dispose();
                    hv_Type.Dispose();
                    hv_NumMatches.Dispose();
                    hv_InputNumChannels.Dispose();
                    hv_OutputNumChannels.Dispose();
                    hv_NumChannels1.Dispose();
                    hv_NumChannels3.Dispose();
                    hv_AreInputNumChannels1.Dispose();
                    hv_AreInputNumChannels3.Dispose();
                    hv_AreInputNumChannels1Or3.Dispose();
                    hv_ValidNumChannels.Dispose();
                    hv_ValidNumChannelsText.Dispose();
                    hv_PreserveDomain.Dispose();
                    hv_Row1.Dispose();
                    hv_Column1.Dispose();
                    hv_Row2.Dispose();
                    hv_Column2.Dispose();
                    hv_UniqRow1.Dispose();
                    hv_UniqColumn1.Dispose();
                    hv_UniqRow2.Dispose();
                    hv_UniqColumn2.Dispose();
                    hv_RectangleIndex.Dispose();
                    hv_OriginalWidth.Dispose();
                    hv_OriginalHeight.Dispose();
                    hv_UniqWidth.Dispose();
                    hv_UniqHeight.Dispose();
                    hv_ScaleWidth.Dispose();
                    hv_ScaleHeight.Dispose();
                    hv_ScaleIndex.Dispose();
                    hv_ImageIndex.Dispose();
                    hv_NumChannels.Dispose();
                    hv_ChannelIndex.Dispose();
                    hv_Min.Dispose();
                    hv_Max.Dispose();
                    hv_Range.Dispose();
                    hv_Scale.Dispose();
                    hv_Shift.Dispose();
                    hv_MeanValues.Dispose();
                    hv_DeviationValues.Dispose();
                    hv_UseDefaultNormalizationValues.Dispose();
                    hv_Exception.Dispose();
                    hv_Indices.Dispose();
                    hv_RescaleRange.Dispose();
                    hv_CurrentNumChannels.Dispose();
                    hv_DiffNumChannelsIndices.Dispose();
                    hv_Index.Dispose();
                    hv_DiffNumChannelsIndex.Dispose();
                    hv_NumDomains.Dispose();
                    hv_DomainIndex.Dispose();

                    return;
                }
                //
                //Validate the number channels of the input images.
                hv_InputNumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_InputNumChannels);
                hv_OutputNumChannels.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_OutputNumChannels = HTuple.TupleGenConst(
                        hv_NumImages, hv_ImageNumChannels);
                }
                //Only for 'image_num_channels' 1 and 3 combinations of 1- and 3-channel images are allowed.
                if ((int)((new HTuple(hv_ImageNumChannels.TupleEqual(1))).TupleOr(new HTuple(hv_ImageNumChannels.TupleEqual(
                    3)))) != 0)
                {
                    hv_NumChannels1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels1 = HTuple.TupleGenConst(
                            hv_NumImages, 1);
                    }
                    hv_NumChannels3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels3 = HTuple.TupleGenConst(
                            hv_NumImages, 3);
                    }
                    hv_AreInputNumChannels1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels1 = hv_InputNumChannels.TupleEqualElem(
                            hv_NumChannels1);
                    }
                    hv_AreInputNumChannels3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels3 = hv_InputNumChannels.TupleEqualElem(
                            hv_NumChannels3);
                    }
                    hv_AreInputNumChannels1Or3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels1Or3 = hv_AreInputNumChannels1 + hv_AreInputNumChannels3;
                    }
                    hv_ValidNumChannels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannels = new HTuple(hv_AreInputNumChannels1Or3.TupleEqual(
                            hv_NumChannels1));
                    }
                    hv_ValidNumChannelsText.Dispose();
                    hv_ValidNumChannelsText = "Valid numbers of channels for the specified model are 1 or 3.";
                }
                else
                {
                    hv_ValidNumChannels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannels = new HTuple(hv_InputNumChannels.TupleEqual(
                            hv_OutputNumChannels));
                    }
                    hv_ValidNumChannelsText.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannelsText = ("Valid number of channels for the specified model is " + hv_ImageNumChannels) + ".";
                    }
                }
                if ((int)(hv_ValidNumChannels.TupleNot()) != 0)
                {
                    throw new HalconException("Please provide images with a valid number of channels. " + hv_ValidNumChannelsText);
                }
                //Preprocess the images.
                //
                //For models of type '3d_gripping_point_detection', the preprocessing steps need to be performed on full
                //domain images while the domains are preserved and set back into the images after the preprocessing.
                hv_PreserveDomain.Dispose();
                hv_PreserveDomain = 0;
                if ((int)((new HTuple(hv_ModelType.TupleEqual("3d_gripping_point_detection"))).TupleAnd(
                    (new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))).TupleOr(new HTuple(hv_DomainHandling.TupleEqual(
                    "keep_domain"))))) != 0)
                {
                    hv_PreserveDomain.Dispose();
                    hv_PreserveDomain = 1;
                    ho_PreservedDomains.Dispose();
                    HOperatorSet.GetDomain(ho_Images_COPY_INP_TMP, out ho_PreservedDomains);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Apply the domain to the images.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    if ((int)(hv_PreserveDomain) != 0)
                    {
                        //In case of preserved domain, the crop is performed with the smallest rectangle of the
                        //domain to avoid out of domain pixels being set to 0.
                        hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                        HOperatorSet.SmallestRectangle1(ho_PreservedDomains, out hv_Row1, out hv_Column1,
                            out hv_Row2, out hv_Column2);
                        hv_UniqRow1.Dispose();
                        HOperatorSet.TupleUniq(hv_Row1, out hv_UniqRow1);
                        hv_UniqColumn1.Dispose();
                        HOperatorSet.TupleUniq(hv_Column1, out hv_UniqColumn1);
                        hv_UniqRow2.Dispose();
                        HOperatorSet.TupleUniq(hv_Row2, out hv_UniqRow2);
                        hv_UniqColumn2.Dispose();
                        HOperatorSet.TupleUniq(hv_Column2, out hv_UniqColumn2);
                        if ((int)((new HTuple((new HTuple((new HTuple((new HTuple(hv_UniqRow1.TupleLength()
                            )).TupleEqual(1))).TupleAnd(new HTuple((new HTuple(hv_UniqColumn1.TupleLength()
                            )).TupleEqual(1))))).TupleAnd(new HTuple((new HTuple(hv_UniqRow2.TupleLength()
                            )).TupleEqual(1))))).TupleAnd(new HTuple((new HTuple(hv_UniqColumn2.TupleLength()
                            )).TupleEqual(1)))) != 0)
                        {
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.CropRectangle1(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                                    hv_UniqRow1, hv_UniqColumn1, hv_UniqRow2, hv_UniqColumn2);
                                ho_Images_COPY_INP_TMP.Dispose();
                                ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.MoveRegion(ho_PreservedDomains, out ExpTmpOutVar_0, -hv_UniqRow1,
                                    -hv_UniqColumn1);
                                ho_PreservedDomains.Dispose();
                                ho_PreservedDomains = ExpTmpOutVar_0;
                            }
                        }
                        else
                        {
                            for (hv_RectangleIndex = 0; (int)hv_RectangleIndex <= (int)((new HTuple(hv_Row1.TupleLength()
                                )) - 1); hv_RectangleIndex = (int)hv_RectangleIndex + 1)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_ImageSelected.Dispose();
                                    HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                        hv_RectangleIndex + 1);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.CropRectangle1(ho_ImageSelected, out ExpTmpOutVar_0, hv_Row1.TupleSelect(
                                        hv_RectangleIndex), hv_Column1.TupleSelect(hv_RectangleIndex),
                                        hv_Row2.TupleSelect(hv_RectangleIndex), hv_Column2.TupleSelect(
                                        hv_RectangleIndex));
                                    ho_ImageSelected.Dispose();
                                    ho_ImageSelected = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                        hv_RectangleIndex + 1);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_DomainSelected.Dispose();
                                    HOperatorSet.SelectObj(ho_PreservedDomains, out ho_DomainSelected,
                                        hv_RectangleIndex + 1);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.MoveRegion(ho_DomainSelected, out ExpTmpOutVar_0, -(hv_Row1.TupleSelect(
                                        hv_RectangleIndex)), -(hv_Column1.TupleSelect(hv_RectangleIndex)));
                                    ho_DomainSelected.Dispose();
                                    ho_DomainSelected = ExpTmpOutVar_0;
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_PreservedDomains, ho_DomainSelected, out ExpTmpOutVar_0,
                                        hv_RectangleIndex + 1);
                                    ho_PreservedDomains.Dispose();
                                    ho_PreservedDomains = ExpTmpOutVar_0;
                                }
                            }
                        }
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.CropDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    (new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))).TupleOr(new HTuple(hv_ModelType.TupleEqual(
                    "3d_gripping_point_detection"))))) != 0)
                {
                    //The option 'keep_domain' is only supported for models of 'type' = 'anomaly_detection' or '3d_gripping_point_detection'.
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'.");
                }
                //
                //Zoom preserved domains before zooming the images.
                if ((int)(hv_PreserveDomain) != 0)
                {
                    hv_OriginalWidth.Dispose(); hv_OriginalHeight.Dispose();
                    HOperatorSet.GetImageSize(ho_Images_COPY_INP_TMP, out hv_OriginalWidth, out hv_OriginalHeight);
                    hv_UniqWidth.Dispose();
                    HOperatorSet.TupleUniq(hv_OriginalWidth, out hv_UniqWidth);
                    hv_UniqHeight.Dispose();
                    HOperatorSet.TupleUniq(hv_OriginalHeight, out hv_UniqHeight);
                    if ((int)((new HTuple((new HTuple(hv_UniqWidth.TupleLength())).TupleEqual(
                        1))).TupleAnd(new HTuple((new HTuple(hv_UniqHeight.TupleLength())).TupleEqual(
                        1)))) != 0)
                    {
                        hv_ScaleWidth.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleWidth = hv_ImageWidth / (hv_UniqWidth.TupleReal()
                                );
                        }
                        hv_ScaleHeight.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleHeight = hv_ImageHeight / (hv_UniqHeight.TupleReal()
                                );
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ZoomRegion(ho_PreservedDomains, out ExpTmpOutVar_0, hv_ScaleWidth,
                                hv_ScaleHeight);
                            ho_PreservedDomains.Dispose();
                            ho_PreservedDomains = ExpTmpOutVar_0;
                        }
                    }
                    else
                    {
                        hv_ScaleWidth.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleWidth = hv_ImageWidth / (hv_OriginalWidth.TupleReal()
                                );
                        }
                        hv_ScaleHeight.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ScaleHeight = hv_ImageHeight / (hv_OriginalHeight.TupleReal()
                                );
                        }
                        for (hv_ScaleIndex = 0; (int)hv_ScaleIndex <= (int)((new HTuple(hv_ScaleWidth.TupleLength()
                            )) - 1); hv_ScaleIndex = (int)hv_ScaleIndex + 1)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_DomainSelected.Dispose();
                                HOperatorSet.SelectObj(ho_PreservedDomains, out ho_DomainSelected, hv_ScaleIndex + 1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ZoomRegion(ho_DomainSelected, out ExpTmpOutVar_0, hv_ScaleWidth.TupleSelect(
                                    hv_ScaleIndex), hv_ScaleHeight.TupleSelect(hv_ScaleIndex));
                                ho_DomainSelected.Dispose();
                                ho_DomainSelected = ExpTmpOutVar_0;
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ReplaceObj(ho_PreservedDomains, ho_DomainSelected, out ExpTmpOutVar_0,
                                    hv_ScaleIndex + 1);
                                ho_PreservedDomains.Dispose();
                                ho_PreservedDomains = ExpTmpOutVar_0;
                            }
                        }
                    }
                }
                //
                //Convert the images to real and zoom the images.
                //Zoom first to speed up if all image types are supported by zoom_image_size.
                if ((int)(new HTuple((new HTuple(hv_Type.TupleRegexpTest("int1|int4|int8"))).TupleEqual(
                    0))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                if ((int)(new HTuple(hv_NormalizationType.TupleEqual("all_channels"))) != 0)
                {
                    //Scale for each image the gray values of all channels to ImageRangeMin-ImageRangeMax.
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val138 = hv_NumImages;
                    HTuple step_val138 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val138, step_val138); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val138))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val142 = hv_NumChannels;
                        HTuple step_val142 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val142, step_val142); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val142))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                            HOperatorSet.MinMaxGray(ho_Channel, ho_Channel, 0, out hv_Min, out hv_Max,
                                out hv_Range);
                            if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                            {
                                hv_Scale.Dispose();
                                hv_Scale = 1;
                            }
                            else
                            {
                                hv_Scale.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                                }
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageScaled, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("first_channel"))) != 0)
                {
                    //Scale for each image the gray values of first channel to ImageRangeMin-ImageRangeMax.
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val160 = hv_NumImages;
                    HTuple step_val160 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val160, step_val160); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val160))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                        HOperatorSet.MinMaxGray(ho_ImageSelected, ho_ImageSelected, 0, out hv_Min,
                            out hv_Max, out hv_Range);
                        if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                        {
                            hv_Scale.Dispose();
                            hv_Scale = 1;
                        }
                        else
                        {
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                            }
                        }
                        hv_Shift.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_Scale,
                                hv_Shift);
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageSelected, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("constant_values"))) != 0)
                {
                    //Scale for each image the gray values of all channels to the corresponding channel DeviationValues[].
                    try
                    {
                        hv_MeanValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "mean_values_normalization",
                            out hv_MeanValues);
                        hv_DeviationValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "deviation_values_normalization",
                            out hv_DeviationValues);
                        hv_UseDefaultNormalizationValues.Dispose();
                        hv_UseDefaultNormalizationValues = 0;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_MeanValues.Dispose();
                        hv_MeanValues = new HTuple();
                        hv_MeanValues[0] = 123.675;
                        hv_MeanValues[1] = 116.28;
                        hv_MeanValues[2] = 103.53;
                        hv_DeviationValues.Dispose();
                        hv_DeviationValues = new HTuple();
                        hv_DeviationValues[0] = 58.395;
                        hv_DeviationValues[1] = 57.12;
                        hv_DeviationValues[2] = 57.375;
                        hv_UseDefaultNormalizationValues.Dispose();
                        hv_UseDefaultNormalizationValues = 1;
                    }
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val185 = hv_NumImages;
                    HTuple step_val185 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val185, step_val185); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val185))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                        //Ensure that the number of channels is equal |DeviationValues| and |MeanValues|
                        if ((int)(hv_UseDefaultNormalizationValues) != 0)
                        {
                            if ((int)(new HTuple(hv_NumChannels.TupleEqual(1))) != 0)
                            {
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Compose3(ho_ImageSelected, ho_ImageSelected, ho_ImageSelected,
                                        out ExpTmpOutVar_0);
                                    ho_ImageSelected.Dispose();
                                    ho_ImageSelected = ExpTmpOutVar_0;
                                }
                                hv_NumChannels.Dispose();
                                HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                            }
                            else if ((int)(new HTuple(hv_NumChannels.TupleNotEqual(
                                3))) != 0)
                            {
                                throw new HalconException("Using default values for normalization type 'constant_values' is allowed only for 1- and 3-channel images.");
                            }
                        }
                        if ((int)((new HTuple((new HTuple(hv_MeanValues.TupleLength())).TupleNotEqual(
                            hv_NumChannels))).TupleOr(new HTuple((new HTuple(hv_DeviationValues.TupleLength()
                            )).TupleNotEqual(hv_NumChannels)))) != 0)
                        {
                            throw new HalconException("The length of mean and deviation values for normalization type 'constant_values' have to be the same size as the number of channels of the image.");
                        }
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val201 = hv_NumChannels;
                        HTuple step_val201 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val201, step_val201); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val201))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = 1.0 / (hv_DeviationValues.TupleSelect(
                                    hv_ChannelIndex - 1));
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = (-hv_Scale) * (hv_MeanValues.TupleSelect(
                                    hv_ChannelIndex - 1));
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageScaled, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("none"))) != 0)
                {
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_Type, "byte", out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        //Shift the gray values from [0-255] to the expected range for byte images.
                        hv_RescaleRange.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RescaleRange = (hv_ImageRangeMax - hv_ImageRangeMin) / 255.0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_Indices + 1);
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_RescaleRange,
                                hv_ImageRangeMin);
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                hv_Indices + 1);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleNotEqual("none"))) != 0)
                {
                    throw new HalconException("Unsupported parameter value for 'normalization_type'");
                }
                //
                //Ensure that the number of channels of the resulting images is consistent with the
                //number of channels of the given model. The only exceptions that are adapted below
                //are combinations of 1- and 3-channel images if ImageNumChannels is either 1 or 3.
                if ((int)((new HTuple(hv_ImageNumChannels.TupleEqual(1))).TupleOr(new HTuple(hv_ImageNumChannels.TupleEqual(
                    3)))) != 0)
                {
                    hv_CurrentNumChannels.Dispose();
                    HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_CurrentNumChannels);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DiffNumChannelsIndices.Dispose();
                        HOperatorSet.TupleFind(hv_CurrentNumChannels.TupleNotEqualElem(hv_OutputNumChannels),
                            1, out hv_DiffNumChannelsIndices);
                    }
                    if ((int)(new HTuple(hv_DiffNumChannelsIndices.TupleNotEqual(-1))) != 0)
                    {
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_DiffNumChannelsIndices.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            hv_DiffNumChannelsIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DiffNumChannelsIndex = hv_DiffNumChannelsIndices.TupleSelect(
                                    hv_Index);
                            }
                            hv_ImageIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageIndex = hv_DiffNumChannelsIndex + 1;
                            }
                            hv_NumChannels.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NumChannels = hv_CurrentNumChannels.TupleSelect(
                                    hv_ImageIndex - 1);
                            }
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                hv_ImageIndex);
                            if ((int)((new HTuple(hv_NumChannels.TupleEqual(1))).TupleAnd(new HTuple(hv_ImageNumChannels.TupleEqual(
                                3)))) != 0)
                            {
                                //Conversion from 1- to 3-channel image required
                                ho_ThreeChannelImage.Dispose();
                                HOperatorSet.Compose3(ho_ImageSelected, ho_ImageSelected, ho_ImageSelected,
                                    out ho_ThreeChannelImage);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ThreeChannelImage,
                                        out ExpTmpOutVar_0, hv_ImageIndex);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                            }
                            else if ((int)((new HTuple(hv_NumChannels.TupleEqual(3))).TupleAnd(
                                new HTuple(hv_ImageNumChannels.TupleEqual(1)))) != 0)
                            {
                                //Conversion from 3- to 1-channel image required
                                ho_SingleChannelImage.Dispose();
                                HOperatorSet.Rgb1ToGray(ho_ImageSelected, out ho_SingleChannelImage
                                    );
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_SingleChannelImage,
                                        out ExpTmpOutVar_0, hv_ImageIndex);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                            }
                            else
                            {
                                throw new HalconException(((("Unexpected error adapting the number of channels. The number of channels of the resulting image is " + hv_NumChannels) + new HTuple(", but the number of channels of the model is ")) + hv_ImageNumChannels) + ".");
                            }
                        }
                    }
                }
                //
                //In case the image domains were preserved, they need to be set back into the images.
                if ((int)(hv_PreserveDomain) != 0)
                {
                    hv_NumDomains.Dispose();
                    HOperatorSet.CountObj(ho_PreservedDomains, out hv_NumDomains);
                    HTuple end_val254 = hv_NumDomains;
                    HTuple step_val254 = 1;
                    for (hv_DomainIndex = 1; hv_DomainIndex.Continue(end_val254, step_val254); hv_DomainIndex = hv_DomainIndex.TupleAdd(step_val254))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_DomainIndex);
                        ho_DomainSelected.Dispose();
                        HOperatorSet.SelectObj(ho_PreservedDomains, out ho_DomainSelected, hv_DomainIndex);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReduceDomain(ho_ImageSelected, ho_DomainSelected, out ExpTmpOutVar_0
                                );
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                hv_DomainIndex);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                //
                //Write preprocessed images to output variable.
                ho_ImagesPreprocessed.Dispose();
                ho_ImagesPreprocessed = new HObject(ho_Images_COPY_INP_TMP);
                //
                ho_Images_COPY_INP_TMP.Dispose();
                ho_PreservedDomains.Dispose();
                ho_ImageSelected.Dispose();
                ho_DomainSelected.Dispose();
                ho_ImagesScaled.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_InputNumChannels.Dispose();
                hv_OutputNumChannels.Dispose();
                hv_NumChannels1.Dispose();
                hv_NumChannels3.Dispose();
                hv_AreInputNumChannels1.Dispose();
                hv_AreInputNumChannels3.Dispose();
                hv_AreInputNumChannels1Or3.Dispose();
                hv_ValidNumChannels.Dispose();
                hv_ValidNumChannelsText.Dispose();
                hv_PreserveDomain.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_UniqRow1.Dispose();
                hv_UniqColumn1.Dispose();
                hv_UniqRow2.Dispose();
                hv_UniqColumn2.Dispose();
                hv_RectangleIndex.Dispose();
                hv_OriginalWidth.Dispose();
                hv_OriginalHeight.Dispose();
                hv_UniqWidth.Dispose();
                hv_UniqHeight.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_ScaleIndex.Dispose();
                hv_ImageIndex.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_UseDefaultNormalizationValues.Dispose();
                hv_Exception.Dispose();
                hv_Indices.Dispose();
                hv_RescaleRange.Dispose();
                hv_CurrentNumChannels.Dispose();
                hv_DiffNumChannelsIndices.Dispose();
                hv_Index.Dispose();
                hv_DiffNumChannelsIndex.Dispose();
                hv_NumDomains.Dispose();
                hv_DomainIndex.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Images_COPY_INP_TMP.Dispose();
                ho_PreservedDomains.Dispose();
                ho_ImageSelected.Dispose();
                ho_DomainSelected.Dispose();
                ho_ImagesScaled.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_InputNumChannels.Dispose();
                hv_OutputNumChannels.Dispose();
                hv_NumChannels1.Dispose();
                hv_NumChannels3.Dispose();
                hv_AreInputNumChannels1.Dispose();
                hv_AreInputNumChannels3.Dispose();
                hv_AreInputNumChannels1Or3.Dispose();
                hv_ValidNumChannels.Dispose();
                hv_ValidNumChannelsText.Dispose();
                hv_PreserveDomain.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_UniqRow1.Dispose();
                hv_UniqColumn1.Dispose();
                hv_UniqRow2.Dispose();
                hv_UniqColumn2.Dispose();
                hv_RectangleIndex.Dispose();
                hv_OriginalWidth.Dispose();
                hv_OriginalHeight.Dispose();
                hv_UniqWidth.Dispose();
                hv_UniqHeight.Dispose();
                hv_ScaleWidth.Dispose();
                hv_ScaleHeight.Dispose();
                hv_ScaleIndex.Dispose();
                hv_ImageIndex.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_UseDefaultNormalizationValues.Dispose();
                hv_Exception.Dispose();
                hv_Indices.Dispose();
                hv_RescaleRange.Dispose();
                hv_CurrentNumChannels.Dispose();
                hv_DiffNumChannelsIndices.Dispose();
                hv_Index.Dispose();
                hv_DiffNumChannelsIndex.Dispose();
                hv_NumDomains.Dispose();
                hv_DomainIndex.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Preprocess images for deep-learning-based training and inference of Deep OCR detection models. 
        public void preprocess_dl_model_images_ocr_detection(HObject ho_Images, out HObject ho_ImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image = null, ho_ImageScaled = null;
            HObject ho_Channel = null, ho_ChannelScaled = null, ho_ImageG = null;
            HObject ho_ImageB = null;

            // Local copy input parameter variables 
            HObject ho_Images_COPY_INP_TMP;
            ho_Images_COPY_INP_TMP = new HObject(ho_Images);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ImageRangeMin = new HTuple();
            HTuple hv_ImageRangeMax = new HTuple(), hv_DomainHandling = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ImageTypes = new HTuple(), hv_InputImageWidths = new HTuple();
            HTuple hv_InputImageHeights = new HTuple(), hv_ImageRange = new HTuple();
            HTuple hv_I = new HTuple(), hv_InputImageWidth = new HTuple();
            HTuple hv_InputImageHeight = new HTuple(), hv_ZoomFactorWidth = new HTuple();
            HTuple hv_ZoomFactorHeight = new HTuple(), hv_ZoomHeight = new HTuple();
            HTuple hv_ZoomWidth = new HTuple(), hv_ChannelIndex = new HTuple();
            HTuple hv_Min = new HTuple(), hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Shift = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagesPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Channel);
            HOperatorSet.GenEmptyObj(out ho_ChannelScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            try
            {
                //This procedure preprocesses the provided images according to the parameters
                //in the dictionary DLPreprocessParam for an ocr_detection model.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_NormalizationType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Check the preprocessing parameters.
                if ((int)(new HTuple(hv_ModelType.TupleNotEqual("ocr_detection"))) != 0)
                {
                    throw new HalconException("The only 'model_type' value supported is'ocr_detection'.");
                }
                if ((int)(new HTuple(hv_ImageNumChannels.TupleNotEqual(3))) != 0)
                {
                    throw new HalconException("The only 'image_num_channels' value supported for ocr_detection models is 3.");
                }
                if ((int)(new HTuple(hv_DomainHandling.TupleNotEqual("full_domain"))) != 0)
                {
                    throw new HalconException("The only 'domain_handling' value supported for ocr_detection models is 'full_domain'.");
                }
                if ((int)((new HTuple(hv_NormalizationType.TupleNotEqual("none"))).TupleAnd(
                    new HTuple(hv_NormalizationType.TupleNotEqual("all_channels")))) != 0)
                {
                    throw new HalconException("The 'normalization_type' values supported for ocr_detection models are 'all_channels' and 'none'.");
                }
                //
                //Get the image properties.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_NumChannels);
                hv_ImageTypes.Dispose();
                HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_ImageTypes);
                hv_InputImageWidths.Dispose(); hv_InputImageHeights.Dispose();
                HOperatorSet.GetImageSize(ho_Images_COPY_INP_TMP, out hv_InputImageWidths,
                    out hv_InputImageHeights);
                //
                //Check the image properties.
                if ((int)(new HTuple(hv_NumImages.TupleEqual(0))) != 0)
                {
                    throw new HalconException("Please provide some images to preprocess.");
                }
                if ((int)(new HTuple(hv_NumImages.TupleNotEqual(new HTuple(hv_ImageTypes.TupleRegexpTest(
                    "byte"))))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte'.");
                }
                if ((int)(new HTuple(hv_NumImages.TupleNotEqual((new HTuple(((hv_NumChannels.TupleEqualElem(
                    1))).TupleOr(hv_NumChannels.TupleEqualElem(3)))).TupleSum()))) != 0)
                {
                    throw new HalconException("Please provide only 1- or 3-channels images for ocr_detection models.");
                }
                //
                //Preprocess the images.
                hv_ImageRange.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageRange = ((hv_ImageRangeMax - hv_ImageRangeMin)).TupleReal()
                        ;
                }
                HTuple end_val49 = hv_NumImages - 1;
                HTuple step_val49 = 1;
                for (hv_I = 0; hv_I.Continue(end_val49, step_val49); hv_I = hv_I.TupleAdd(step_val49))
                {
                    hv_InputImageWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageWidth = hv_InputImageWidths.TupleSelect(
                            hv_I);
                    }
                    hv_InputImageHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageHeight = hv_InputImageHeights.TupleSelect(
                            hv_I);
                    }
                    //
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_Image, hv_I + 1);
                    }
                    //
                    //Calculate aspect-ratio preserving zoom factors
                    hv_ZoomFactorWidth.Dispose(); hv_ZoomFactorHeight.Dispose();
                    calculate_dl_image_zoom_factors(hv_InputImageWidth, hv_InputImageHeight,
                        hv_ImageWidth, hv_ImageHeight, hv_DLPreprocessParam, out hv_ZoomFactorWidth,
                        out hv_ZoomFactorHeight);
                    //
                    //Zoom image
                    hv_ZoomHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomHeight = ((hv_ZoomFactorHeight * hv_InputImageHeight)).TupleRound()
                            ;
                    }
                    hv_ZoomWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomWidth = ((hv_ZoomFactorWidth * hv_InputImageWidth)).TupleRound()
                            ;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Image, out ExpTmpOutVar_0, hv_ZoomWidth, hv_ZoomHeight,
                            "constant");
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    //
                    //Convert to real and normalize
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Image, out ExpTmpOutVar_0, "real");
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_NormalizationType.TupleEqual("all_channels"))) != 0)
                    {
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val67 = hv_NumChannels.TupleSelect(
                            hv_I);
                        HTuple step_val67 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val67, step_val67); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val67))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_Image, out ho_Channel, hv_ChannelIndex);
                            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                            HOperatorSet.MinMaxGray(ho_Channel, ho_Channel, 0, out hv_Min, out hv_Max,
                                out hv_Range);
                            if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                            {
                                hv_Scale.Dispose();
                                hv_Scale = 1;
                            }
                            else
                            {
                                hv_Scale.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                                }
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        ho_Image.Dispose();
                        ho_Image = new HObject(ho_ImageScaled);
                    }
                    else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("none"))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_Image, out ExpTmpOutVar_0, hv_ImageRange / 255.0,
                                hv_ImageRangeMin);
                            ho_Image.Dispose();
                            ho_Image = ExpTmpOutVar_0;
                        }
                    }
                    //
                    //Obtain an RGB image.
                    if ((int)(new HTuple(((hv_NumChannels.TupleSelect(hv_I))).TupleEqual(1))) != 0)
                    {
                        ho_ImageG.Dispose();
                        HOperatorSet.CopyImage(ho_Image, out ho_ImageG);
                        ho_ImageB.Dispose();
                        HOperatorSet.CopyImage(ho_Image, out ho_ImageB);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Compose3(ho_Image, ho_ImageG, ho_ImageB, out ExpTmpOutVar_0
                                );
                            ho_Image.Dispose();
                            ho_Image = ExpTmpOutVar_0;
                        }
                    }
                    //
                    //Apply padding to fit the desired image size.
                    //The padding value is zero, corresponding to the
                    //border handling of the convolution layers.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ChangeFormat(ho_Image, out ExpTmpOutVar_0, hv_ImageWidth, hv_ImageHeight);
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_Image, out ExpTmpOutVar_0,
                            hv_I + 1);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Return the preprocessed images.
                ho_ImagesPreprocessed.Dispose();
                ho_ImagesPreprocessed = new HObject(ho_Images_COPY_INP_TMP);
                ho_Images_COPY_INP_TMP.Dispose();
                ho_Image.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageTypes.Dispose();
                hv_InputImageWidths.Dispose();
                hv_InputImageHeights.Dispose();
                hv_ImageRange.Dispose();
                hv_I.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_ZoomFactorWidth.Dispose();
                hv_ZoomFactorHeight.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Images_COPY_INP_TMP.Dispose();
                ho_Image.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ImageG.Dispose();
                ho_ImageB.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageTypes.Dispose();
                hv_InputImageWidths.Dispose();
                hv_InputImageHeights.Dispose();
                hv_ImageRange.Dispose();
                hv_I.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_ZoomFactorWidth.Dispose();
                hv_ZoomFactorHeight.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Preprocess images for deep-learning-based training and inference of Deep OCR recognition models. 
        public void preprocess_dl_model_images_ocr_recognition(HObject ho_Images, out HObject ho_ImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_TargetImage, ho_Image = null;

            // Local copy input parameter variables 
            HObject ho_Images_COPY_INP_TMP;
            ho_Images_COPY_INP_TMP = new HObject(ho_Images);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ImageRangeMin = new HTuple();
            HTuple hv_ImageRangeMax = new HTuple(), hv_DomainHandling = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ImageTypes = new HTuple(), hv_InputImageWidths = new HTuple();
            HTuple hv_InputImageHeights = new HTuple(), hv_PaddingGrayval = new HTuple();
            HTuple hv_ImageRange = new HTuple(), hv_I = new HTuple();
            HTuple hv_InputImageWidth = new HTuple(), hv_InputImageHeight = new HTuple();
            HTuple hv_InputImageWidthHeightRatio = new HTuple(), hv_ZoomHeight = new HTuple();
            HTuple hv_ZoomWidth = new HTuple(), hv_GrayvalMin = new HTuple();
            HTuple hv_GrayvalMax = new HTuple(), hv_Range = new HTuple();
            HTuple hv_GrayvalRange = new HTuple(), hv_Scale = new HTuple();
            HTuple hv_Shift = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagesPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_TargetImage);
            HOperatorSet.GenEmptyObj(out ho_Image);
            try
            {
                //This procedure preprocesses the provided Images according to the parameters
                //in the dictionary DLPreprocessParam for an ocr_recognition model.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_NormalizationType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Check the preprocessing parameters.
                if ((int)(new HTuple(hv_ModelType.TupleNotEqual("ocr_recognition"))) != 0)
                {
                    throw new HalconException("The only 'model_type' value supported is'ocr_recognition'.");
                }
                if ((int)(new HTuple(hv_ImageNumChannels.TupleNotEqual(1))) != 0)
                {
                    throw new HalconException("The only 'image_num_channels' value supported for ocr_recognition models is 1.");
                }
                if ((int)(new HTuple(hv_DomainHandling.TupleNotEqual("full_domain"))) != 0)
                {
                    throw new HalconException("The only 'domain_handling' value supported for ocr_recognition models is 'full_domain'.");
                }
                if ((int)((new HTuple((new HTuple(hv_NormalizationType.TupleNotEqual("none"))).TupleAnd(
                    new HTuple(hv_NormalizationType.TupleNotEqual("first_channel"))))).TupleAnd(
                    new HTuple(hv_NormalizationType.TupleNotEqual("all_channels")))) != 0)
                {
                    throw new HalconException(new HTuple("The 'normalization_type' values supported for ocr_recognition models are 'first_channel', 'all_channels' and 'none'."));
                }
                //
                //Get the image properties.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                hv_NumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_NumChannels);
                hv_ImageTypes.Dispose();
                HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_ImageTypes);
                hv_InputImageWidths.Dispose(); hv_InputImageHeights.Dispose();
                HOperatorSet.GetImageSize(ho_Images_COPY_INP_TMP, out hv_InputImageWidths,
                    out hv_InputImageHeights);
                //
                //Check the image properties.
                if ((int)(new HTuple(hv_NumImages.TupleEqual(0))) != 0)
                {
                    throw new HalconException("Please provide some images to preprocess.");
                }
                if ((int)(new HTuple(hv_NumImages.TupleNotEqual(new HTuple(hv_ImageTypes.TupleRegexpTest(
                    "byte|real"))))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte' or 'real'.");
                }
                if ((int)(new HTuple(hv_NumImages.TupleNotEqual((new HTuple(((hv_NumChannels.TupleEqualElem(
                    1))).TupleOr(hv_NumChannels.TupleEqualElem(3)))).TupleSum()))) != 0)
                {
                    throw new HalconException("Please provide only 1- or 3-channels images for ocr_recognition models.");
                }
                //
                //Preprocess the images.
                hv_PaddingGrayval.Dispose();
                hv_PaddingGrayval = 0.0;
                hv_ImageRange.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ImageRange = ((hv_ImageRangeMax - hv_ImageRangeMin)).TupleReal()
                        ;
                }
                ho_TargetImage.Dispose();
                HOperatorSet.GenImageConst(out ho_TargetImage, "real", hv_ImageWidth, hv_ImageHeight);
                HOperatorSet.OverpaintRegion(ho_TargetImage, ho_TargetImage, hv_PaddingGrayval,
                    "fill");
                HTuple end_val52 = hv_NumImages - 1;
                HTuple step_val52 = 1;
                for (hv_I = 0; hv_I.Continue(end_val52, step_val52); hv_I = hv_I.TupleAdd(step_val52))
                {
                    hv_InputImageWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageWidth = hv_InputImageWidths.TupleSelect(
                            hv_I);
                    }
                    hv_InputImageHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageHeight = hv_InputImageHeights.TupleSelect(
                            hv_I);
                    }
                    hv_InputImageWidthHeightRatio.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_InputImageWidthHeightRatio = hv_InputImageWidth / (hv_InputImageHeight.TupleReal()
                            );
                    }
                    //
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_Image, hv_I + 1);
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Image, out ExpTmpOutVar_0);
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(((hv_NumChannels.TupleSelect(hv_I))).TupleEqual(3))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Rgb1ToGray(ho_Image, out ExpTmpOutVar_0);
                            ho_Image.Dispose();
                            ho_Image = ExpTmpOutVar_0;
                        }
                    }
                    //
                    hv_ZoomHeight.Dispose();
                    hv_ZoomHeight = new HTuple(hv_ImageHeight);
                    hv_ZoomWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ZoomWidth = hv_ImageWidth.TupleMin2(
                            ((hv_ImageHeight * hv_InputImageWidthHeightRatio)).TupleInt());
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Image, out ExpTmpOutVar_0, hv_ZoomWidth, hv_ZoomHeight,
                            "constant");
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(((hv_ImageTypes.TupleSelect(hv_I))).TupleEqual("byte"))) != 0)
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConvertImageType(ho_Image, out ExpTmpOutVar_0, "real");
                            ho_Image.Dispose();
                            ho_Image = ExpTmpOutVar_0;
                        }
                    }
                    if ((int)((new HTuple(hv_NormalizationType.TupleEqual("first_channel"))).TupleOr(
                        new HTuple(hv_NormalizationType.TupleEqual("all_channels")))) != 0)
                    {
                        hv_GrayvalMin.Dispose(); hv_GrayvalMax.Dispose(); hv_Range.Dispose();
                        HOperatorSet.MinMaxGray(ho_Image, ho_Image, 0, out hv_GrayvalMin, out hv_GrayvalMax,
                            out hv_Range);
                        hv_GrayvalRange.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GrayvalRange = ((hv_GrayvalMax - hv_GrayvalMin)).TupleReal()
                                ;
                        }
                        if ((int)(new HTuple(hv_GrayvalRange.TupleEqual(0.0))) != 0)
                        {
                            hv_Scale.Dispose();
                            hv_Scale = 1.0;
                        }
                        else
                        {
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = hv_ImageRange / hv_GrayvalRange;
                            }
                        }
                        hv_Shift.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Shift = ((-hv_Scale) * hv_GrayvalMin) + hv_ImageRangeMin;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_Image, out ExpTmpOutVar_0, hv_Scale, hv_Shift);
                            ho_Image.Dispose();
                            ho_Image = ExpTmpOutVar_0;
                        }
                    }
                    else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("none"))) != 0)
                    {
                        if ((int)(new HTuple(((hv_ImageTypes.TupleSelect(hv_I))).TupleEqual("byte"))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.ScaleImage(ho_Image, out ExpTmpOutVar_0, hv_ImageRange / 255.0,
                                    hv_ImageRangeMin);
                                ho_Image.Dispose();
                                ho_Image = ExpTmpOutVar_0;
                            }
                        }
                    }
                    //
                    HOperatorSet.OverpaintGray(ho_TargetImage, ho_Image);
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReduceDomain(ho_TargetImage, ho_Image, out ExpTmpOutVar_0);
                        ho_TargetImage.Dispose();
                        ho_TargetImage = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_TargetImage, out ExpTmpOutVar_0,
                            hv_I + 1);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Return the preprocessed images.
                ho_ImagesPreprocessed.Dispose();
                ho_ImagesPreprocessed = new HObject(ho_Images_COPY_INP_TMP);
                ho_Images_COPY_INP_TMP.Dispose();
                ho_TargetImage.Dispose();
                ho_Image.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageTypes.Dispose();
                hv_InputImageWidths.Dispose();
                hv_InputImageHeights.Dispose();
                hv_PaddingGrayval.Dispose();
                hv_ImageRange.Dispose();
                hv_I.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_InputImageWidthHeightRatio.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_GrayvalMin.Dispose();
                hv_GrayvalMax.Dispose();
                hv_Range.Dispose();
                hv_GrayvalRange.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Images_COPY_INP_TMP.Dispose();
                ho_TargetImage.Dispose();
                ho_Image.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_NumChannels.Dispose();
                hv_ImageTypes.Dispose();
                hv_InputImageWidths.Dispose();
                hv_InputImageHeights.Dispose();
                hv_PaddingGrayval.Dispose();
                hv_ImageRange.Dispose();
                hv_I.Dispose();
                hv_InputImageWidth.Dispose();
                hv_InputImageHeight.Dispose();
                hv_InputImageWidthHeightRatio.Dispose();
                hv_ZoomHeight.Dispose();
                hv_ZoomWidth.Dispose();
                hv_GrayvalMin.Dispose();
                hv_GrayvalMax.Dispose();
                hv_Range.Dispose();
                hv_GrayvalRange.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection and Instance Segmentation
        // Short Description: Preprocess the instance segmentation masks for a sample given by the dictionary DLSample. 
        private void preprocess_dl_model_instance_masks(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_InstanceMasks, ho_Domain = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_NumMasks = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_DomainRow1 = new HTuple(), hv_DomainColumn1 = new HTuple();
            HTuple hv_DomainRow2 = new HTuple(), hv_DomainColumn2 = new HTuple();
            HTuple hv_FactorResampleWidth = new HTuple(), hv_FactorResampleHeight = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_InstanceMasks);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            try
            {
                //
                //This procedure preprocesses the instance masks of a DLSample.
                //
                //Check preprocess parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get relevant preprocess parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //
                //Get the preprocessed instance masks.
                ho_InstanceMasks.Dispose();
                HOperatorSet.GetDictObject(out ho_InstanceMasks, hv_DLSample, "mask");
                //
                //Get the number of instance masks.
                hv_NumMasks.Dispose();
                HOperatorSet.CountObj(ho_InstanceMasks, out hv_NumMasks);
                //
                //Domain handling of the image to be preprocessed.
                //
                hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //Clip and translate masks w.r.t. the image domain
                    ho_Domain.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_Domain);
                    hv_DomainRow1.Dispose(); hv_DomainColumn1.Dispose(); hv_DomainRow2.Dispose(); hv_DomainColumn2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_Domain, out hv_DomainRow1, out hv_DomainColumn1,
                        out hv_DomainRow2, out hv_DomainColumn2);
                    //
                    //Clip the remaining regions to the domain.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ClipRegion(ho_InstanceMasks, out ExpTmpOutVar_0, hv_DomainRow1,
                            hv_DomainColumn1, hv_DomainRow2, hv_DomainColumn2);
                        ho_InstanceMasks.Dispose();
                        ho_InstanceMasks = ExpTmpOutVar_0;
                    }
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_DomainColumn2 - hv_DomainColumn1) + 1.0;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_DomainRow2 - hv_DomainRow1) + 1.0;
                    }
                    //We need to move the remaining regions back to the origin,
                    //because crop_domain will be applied to the image
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MoveRegion(ho_InstanceMasks, out ExpTmpOutVar_0, -hv_DomainRow1,
                            -hv_DomainColumn1);
                        ho_InstanceMasks.Dispose();
                        ho_InstanceMasks = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleNotEqual("full_domain"))) != 0)
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Zoom masks only if the image has a different size than the specified size.
                if ((int)(((hv_ImageWidth.TupleNotEqualElem(hv_WidthRaw))).TupleOr(hv_ImageHeight.TupleNotEqualElem(
                    hv_HeightRaw))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }

                    //Zoom the masks.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomRegion(ho_InstanceMasks, out ExpTmpOutVar_0, hv_FactorResampleWidth,
                            hv_FactorResampleHeight);
                        ho_InstanceMasks.Dispose();
                        ho_InstanceMasks = ExpTmpOutVar_0;
                    }
                }
                //
                //Set the preprocessed instance masks.
                HOperatorSet.SetDictObject(ho_InstanceMasks, hv_DLSample, "mask");
                //
                //
                ho_InstanceMasks.Dispose();
                ho_Domain.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_NumMasks.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_InstanceMasks.Dispose();
                ho_Domain.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_NumMasks.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Semantic Segmentation and Edge Extraction
        // Short Description: Preprocess segmentation and weight images for deep-learning-based segmentation training and inference. 
        public void preprocess_dl_model_segmentations(HObject ho_ImagesRaw, HObject ho_Segmentations,
            out HObject ho_SegmentationsPreprocessed, HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Domain = null, ho_SelectedSeg = null;
            HObject ho_SelectedDomain = null;

            // Local copy input parameter variables 
            HObject ho_Segmentations_COPY_INP_TMP;
            ho_Segmentations_COPY_INP_TMP = new HObject(ho_Segmentations);



            // Local control variables 

            HTuple hv_NumberImages = new HTuple(), hv_NumberSegmentations = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_WidthSeg = new HTuple(), hv_HeightSeg = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ImageNumChannels = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassesToBackground = new HTuple(), hv_IgnoreClassIDs = new HTuple();
            HTuple hv_IsInt = new HTuple(), hv_IndexImage = new HTuple();
            HTuple hv_ImageWidthRaw = new HTuple(), hv_ImageHeightRaw = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_EqualReal = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationsPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            HOperatorSet.GenEmptyObj(out ho_SelectedSeg);
            HOperatorSet.GenEmptyObj(out ho_SelectedDomain);
            try
            {
                //
                //This procedure preprocesses the segmentation or weight images
                //given by Segmentations so that they can be handled by
                //train_dl_model_batch and apply_dl_model.
                //
                //Check input data.
                //Examine number of images.
                hv_NumberImages.Dispose();
                HOperatorSet.CountObj(ho_ImagesRaw, out hv_NumberImages);
                hv_NumberSegmentations.Dispose();
                HOperatorSet.CountObj(ho_Segmentations_COPY_INP_TMP, out hv_NumberSegmentations);
                if ((int)(new HTuple(hv_NumberImages.TupleNotEqual(hv_NumberSegmentations))) != 0)
                {
                    throw new HalconException("Equal number of images given in ImagesRaw and Segmentations required");
                }
                //Size of images.
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_ImagesRaw, out hv_Width, out hv_Height);
                hv_WidthSeg.Dispose(); hv_HeightSeg.Dispose();
                HOperatorSet.GetImageSize(ho_Segmentations_COPY_INP_TMP, out hv_WidthSeg, out hv_HeightSeg);
                if ((int)((new HTuple(hv_Width.TupleNotEqual(hv_WidthSeg))).TupleOr(new HTuple(hv_Height.TupleNotEqual(
                    hv_HeightSeg)))) != 0)
                {
                    throw new HalconException("Equal size of the images given in ImagesRaw and Segmentations required.");
                }
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the relevant preprocessing parameters.
                hv_DLModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //Segmentation specific parameters.
                hv_SetBackgroundID.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                hv_ClassesToBackground.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassesToBackground);
                hv_IgnoreClassIDs.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                //
                //Check the input parameter for setting the background ID.
                if ((int)(new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))) != 0)
                {
                    //Check that the model is a segmentation model.
                    if ((int)(new HTuple(hv_DLModelType.TupleNotEqual("segmentation"))) != 0)
                    {
                        throw new HalconException("Setting class IDs to background is only implemented for segmentation.");
                    }
                    //Check the background ID.
                    hv_IsInt.Dispose();
                    HOperatorSet.TupleIsIntElem(hv_SetBackgroundID, out hv_IsInt);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    else if ((int)(hv_IsInt.TupleNot()) != 0)
                    {
                        //Given class_id has to be of type int.
                        throw new HalconException("The class_id given as 'set_background_id' has to be of type int.");
                    }
                    //Check the values of ClassesToBackground.
                    if ((int)(new HTuple((new HTuple(hv_ClassesToBackground.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        //Check that the given classes are of length > 0.
                        throw new HalconException(new HTuple("If 'set_background_id' is given, 'class_ids_background' must at least contain this class ID."));
                    }
                    else if ((int)(new HTuple(((hv_ClassesToBackground.TupleIntersection(
                        hv_IgnoreClassIDs))).TupleNotEqual(new HTuple()))) != 0)
                    {
                        //Check that class_ids_background is not included in the ignore_class_ids of the DLModel.
                        throw new HalconException("The given 'class_ids_background' must not be included in the 'ignore_class_ids' of the model.");
                    }
                }
                //
                //Domain handling of the image to be preprocessed.
                //
                if ((int)((new HTuple(hv_DomainHandling.TupleEqual("full_domain"))).TupleOr(
                    new HTuple(hv_DomainHandling.TupleEqual("keep_domain")))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //If the domain should be cropped the domain has to be transferred
                    //from the raw image to the segmentation image.
                    ho_Domain.Dispose();
                    HOperatorSet.GetDomain(ho_ImagesRaw, out ho_Domain);
                    HTuple end_val66 = hv_NumberImages;
                    HTuple step_val66 = 1;
                    for (hv_IndexImage = 1; hv_IndexImage.Continue(end_val66, step_val66); hv_IndexImage = hv_IndexImage.TupleAdd(step_val66))
                    {
                        ho_SelectedSeg.Dispose();
                        HOperatorSet.SelectObj(ho_Segmentations_COPY_INP_TMP, out ho_SelectedSeg,
                            hv_IndexImage);
                        ho_SelectedDomain.Dispose();
                        HOperatorSet.SelectObj(ho_Domain, out ho_SelectedDomain, hv_IndexImage);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ChangeDomain(ho_SelectedSeg, ho_SelectedDomain, out ExpTmpOutVar_0
                                );
                            ho_SelectedSeg.Dispose();
                            ho_SelectedSeg = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Segmentations_COPY_INP_TMP, ho_SelectedSeg,
                                out ExpTmpOutVar_0, hv_IndexImage);
                            ho_Segmentations_COPY_INP_TMP.Dispose();
                            ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Preprocess the segmentation images.
                //
                //Set all background classes to the given background class ID.
                if ((int)(new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        reassign_pixel_values(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ClassesToBackground, hv_SetBackgroundID);
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthRaw.Dispose(); hv_ImageHeightRaw.Dispose();
                HOperatorSet.GetImageSize(ho_Segmentations_COPY_INP_TMP, out hv_ImageWidthRaw,
                    out hv_ImageHeightRaw);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthRaw);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightRaw);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images
                //and convert if necessary.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Segmentations_COPY_INP_TMP, out hv_Type);
                hv_EqualReal.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualReal = hv_Type.TupleEqualElem(
                        "real");
                }
                //
                if ((int)(new HTuple(((hv_EqualReal.TupleMin())).TupleEqual(0))) != 0)
                {
                    //Convert the image type to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Write preprocessed Segmentations to output variable.
                ho_SegmentationsPreprocessed.Dispose();
                ho_SegmentationsPreprocessed = new HObject(ho_Segmentations_COPY_INP_TMP);
                ho_Segmentations_COPY_INP_TMP.Dispose();
                ho_Domain.Dispose();
                ho_SelectedSeg.Dispose();
                ho_SelectedDomain.Dispose();

                hv_NumberImages.Dispose();
                hv_NumberSegmentations.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WidthSeg.Dispose();
                hv_HeightSeg.Dispose();
                hv_DLModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassesToBackground.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_IsInt.Dispose();
                hv_IndexImage.Dispose();
                hv_ImageWidthRaw.Dispose();
                hv_ImageHeightRaw.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_EqualReal.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Segmentations_COPY_INP_TMP.Dispose();
                ho_Domain.Dispose();
                ho_SelectedSeg.Dispose();
                ho_SelectedDomain.Dispose();

                hv_NumberImages.Dispose();
                hv_NumberSegmentations.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WidthSeg.Dispose();
                hv_HeightSeg.Dispose();
                hv_DLModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassesToBackground.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_IsInt.Dispose();
                hv_IndexImage.Dispose();
                hv_ImageWidthRaw.Dispose();
                hv_ImageHeightRaw.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_EqualReal.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess given DLSamples according to the preprocessing parameters given in DLPreprocessParam. 
        public void preprocess_dl_samples(HTuple hv_DLSampleBatch, HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            HObject ho_ImageRaw = null, ho_ImagePreprocessed = null;
            HObject ho_AnomalyImageRaw = null, ho_AnomalyImagePreprocessed = null;
            HObject ho_SegmentationRaw = null, ho_SegmentationPreprocessed = null;
            HObject ho_ImageRawDomain = null;

            // Local control variables 

            HTuple hv_SampleIndex = new HTuple(), hv_DLSample = new HTuple();
            HTuple hv_ImageExists = new HTuple(), hv_KeysExists = new HTuple();
            HTuple hv_AnomalyParamExist = new HTuple(), hv_Rectangle1ParamExist = new HTuple();
            HTuple hv_Rectangle2ParamExist = new HTuple(), hv_InstanceMaskParamExist = new HTuple();
            HTuple hv_SegmentationParamExist = new HTuple(), hv_OCRParamExist = new HTuple();
            HTuple hv_DLPreprocessParam_COPY_INP_TMP = new HTuple(hv_DLPreprocessParam);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageRaw);
            HOperatorSet.GenEmptyObj(out ho_ImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImageRaw);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_SegmentationRaw);
            HOperatorSet.GenEmptyObj(out ho_SegmentationPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_ImageRawDomain);
            try
            {
                //
                //This procedure preprocesses all images of the sample dictionaries
                //in the tuple DLSampleBatch.
                //The images are preprocessed according to the parameters provided
                //in DLPreprocessParam.
                //
                //Check the validity of the preprocessing parameters.
                //The procedure check_dl_preprocess_param might change DLPreprocessParam.
                //To avoid race conditions when preprocess_dl_samples is used from
                //multiple threads with the same DLPreprocessParam dictionary,
                //work on a copy.
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.CopyDict(hv_DLPreprocessParam_COPY_INP_TMP, new HTuple(), new HTuple(),
                        out ExpTmpOutVar_0);
                    hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                    hv_DLPreprocessParam_COPY_INP_TMP = ExpTmpOutVar_0;
                }
                check_dl_preprocess_param(hv_DLPreprocessParam_COPY_INP_TMP);
                //
                //
                //
                //Preprocess the sample entries.
                //
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_DLSampleBatch.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    hv_DLSample.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLSample = hv_DLSampleBatch.TupleSelect(
                            hv_SampleIndex);
                    }
                    //
                    //Preprocess augmentation data.
                    preprocess_dl_model_augmentation_data(hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                    //
                    //Check the existence of the sample keys.
                    hv_ImageExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "image", out hv_ImageExists);
                    //
                    //Preprocess the images.
                    if ((int)(hv_ImageExists) != 0)
                    {
                        //
                        //Get the image.
                        ho_ImageRaw.Dispose();
                        HOperatorSet.GetDictObject(out ho_ImageRaw, hv_DLSample, "image");
                        //
                        //Preprocess the image.
                        ho_ImagePreprocessed.Dispose();
                        preprocess_dl_model_images(ho_ImageRaw, out ho_ImagePreprocessed, hv_DLPreprocessParam_COPY_INP_TMP);
                        //
                        //Replace the image in the dictionary.
                        HOperatorSet.SetDictObject(ho_ImagePreprocessed, hv_DLSample, "image");
                        //
                        //Check existence of model specific sample keys:
                        //- 'anomaly_ground_truth':
                        //  For model 'type' = 'anomaly_detection' and
                        //  model 'type' = 'gc_anomaly_detection'
                        //- 'bbox_row1':
                        //  For 'instance_type' = 'rectangle1' and
                        //  model 'type' = 'detection'
                        //- 'bbox_phi':
                        //  For 'instance_type' = 'rectangle2' and
                        //  model 'type' = 'detection'
                        //- 'mask':
                        //  For 'instance_type' = 'rectangle1',
                        //  model 'type' = 'detection', and
                        //  'instance_segmentation' = true
                        //- 'segmentation_image':
                        //  For model 'type' = 'segmentation'
                        hv_KeysExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSample, "key_exists", (((((new HTuple("anomaly_ground_truth")).TupleConcat(
                            "bbox_row1")).TupleConcat("bbox_phi")).TupleConcat("mask")).TupleConcat(
                            "segmentation_image")).TupleConcat("word"), out hv_KeysExists);
                        hv_AnomalyParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyParamExist = hv_KeysExists.TupleSelect(
                                0);
                        }
                        hv_Rectangle1ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle1ParamExist = hv_KeysExists.TupleSelect(
                                1);
                        }
                        hv_Rectangle2ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle2ParamExist = hv_KeysExists.TupleSelect(
                                2);
                        }
                        hv_InstanceMaskParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceMaskParamExist = hv_KeysExists.TupleSelect(
                                3);
                        }
                        hv_SegmentationParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SegmentationParamExist = hv_KeysExists.TupleSelect(
                                4);
                        }
                        hv_OCRParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_OCRParamExist = hv_KeysExists.TupleSelect(
                                5);
                        }
                        //
                        //Preprocess the anomaly ground truth for
                        //model 'type' = 'anomaly_detection' or
                        //model 'type' = 'gc_anomaly_detection' if present.
                        if ((int)(hv_AnomalyParamExist) != 0)
                        {
                            //
                            //Get the anomaly image.
                            ho_AnomalyImageRaw.Dispose();
                            HOperatorSet.GetDictObject(out ho_AnomalyImageRaw, hv_DLSample, "anomaly_ground_truth");
                            //
                            //Preprocess the anomaly image.
                            ho_AnomalyImagePreprocessed.Dispose();
                            preprocess_dl_model_anomaly(ho_AnomalyImageRaw, out ho_AnomalyImagePreprocessed,
                                hv_DLPreprocessParam_COPY_INP_TMP);
                            //
                            //Set preprocessed anomaly image.
                            HOperatorSet.SetDictObject(ho_AnomalyImagePreprocessed, hv_DLSample,
                                "anomaly_ground_truth");
                        }
                        //
                        //Preprocess depending on the model type.
                        //If bounding boxes are given, rescale them as well.
                        if ((int)(hv_Rectangle1ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle1'.
                            preprocess_dl_model_bbox_rect1(ho_ImageRaw, hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        else if ((int)(hv_Rectangle2ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle2'.
                            preprocess_dl_model_bbox_rect2(ho_ImageRaw, hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        if ((int)(hv_InstanceMaskParamExist) != 0)
                        {
                            //
                            //Preprocess the instance masks.
                            preprocess_dl_model_instance_masks(ho_ImageRaw, hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        //
                        //Preprocess the segmentation image if present.
                        if ((int)(hv_SegmentationParamExist) != 0)
                        {
                            //
                            //Get the segmentation image.
                            ho_SegmentationRaw.Dispose();
                            HOperatorSet.GetDictObject(out ho_SegmentationRaw, hv_DLSample, "segmentation_image");
                            //
                            //Preprocess the segmentation image.
                            ho_SegmentationPreprocessed.Dispose();
                            preprocess_dl_model_segmentations(ho_ImageRaw, ho_SegmentationRaw, out ho_SegmentationPreprocessed,
                                hv_DLPreprocessParam_COPY_INP_TMP);
                            //
                            //Set preprocessed segmentation image.
                            HOperatorSet.SetDictObject(ho_SegmentationPreprocessed, hv_DLSample,
                                "segmentation_image");
                        }
                        //
                        //Preprocess the word bounding boxes and generate targets.
                        if ((int)(hv_OCRParamExist.TupleAnd(hv_Rectangle2ParamExist)) != 0)
                        {
                            //
                            //Preprocess Sample.
                            gen_dl_ocr_detection_targets(hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                        //
                        //Preprocess 3D relevant data if present.
                        hv_KeysExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSample, "key_exists", (((new HTuple("x")).TupleConcat(
                            "y")).TupleConcat("z")).TupleConcat("normals"), out hv_KeysExists);
                        if ((int)(hv_KeysExists.TupleMax()) != 0)
                        {
                            //We need to handle crop_domain before preprocess_dl_model_3d_data
                            //if it is necessary.
                            //Note, we are not cropping the image of DLSample because it has
                            //been done by preprocess_dl_model_images.
                            //Since we always keep the domain of 3D data we do not need to handle
                            //'keep_domain' or 'full_domain'.
                            ho_ImageRawDomain.Dispose();
                            HOperatorSet.GetDomain(ho_ImageRaw, out ho_ImageRawDomain);
                            crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "x", hv_DLPreprocessParam_COPY_INP_TMP);
                            crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "y", hv_DLPreprocessParam_COPY_INP_TMP);
                            crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "z", hv_DLPreprocessParam_COPY_INP_TMP);
                            crop_dl_sample_image(ho_ImageRawDomain, hv_DLSample, "normals", hv_DLPreprocessParam_COPY_INP_TMP);
                            //
                            preprocess_dl_model_3d_data(hv_DLSample, hv_DLPreprocessParam_COPY_INP_TMP);
                        }
                    }
                    else
                    {
                        throw new HalconException((new HTuple("All samples processed need to include an image, but the sample with index ") + hv_SampleIndex) + " does not.");
                    }
                }
                //
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();
                ho_ImageRawDomain.Dispose();

                hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_InstanceMaskParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();
                hv_OCRParamExist.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();
                ho_ImageRawDomain.Dispose();

                hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                hv_SampleIndex.Dispose();
                hv_DLSample.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_InstanceMaskParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();
                hv_OCRParamExist.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Image / Manipulation
        // Short Description: Change value of ValuesToChange in Image to NewValue. 
        private void reassign_pixel_values(HObject ho_Image, out HObject ho_ImageOut,
            HTuple hv_ValuesToChange, HTuple hv_NewValue)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionToChange, ho_RegionClass = null;

            // Local control variables 

            HTuple hv_IndexReset = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageOut);
            HOperatorSet.GenEmptyObj(out ho_RegionToChange);
            HOperatorSet.GenEmptyObj(out ho_RegionClass);
            try
            {
                //
                //This procedure sets all pixels of Image
                //with the values given in ValuesToChange to the given value NewValue.
                //
                ho_RegionToChange.Dispose();
                HOperatorSet.GenEmptyRegion(out ho_RegionToChange);
                for (hv_IndexReset = 0; (int)hv_IndexReset <= (int)((new HTuple(hv_ValuesToChange.TupleLength()
                    )) - 1); hv_IndexReset = (int)hv_IndexReset + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionClass.Dispose();
                        HOperatorSet.Threshold(ho_Image, out ho_RegionClass, hv_ValuesToChange.TupleSelect(
                            hv_IndexReset), hv_ValuesToChange.TupleSelect(hv_IndexReset));
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Union2(ho_RegionToChange, ho_RegionClass, out ExpTmpOutVar_0
                            );
                        ho_RegionToChange.Dispose();
                        ho_RegionToChange = ExpTmpOutVar_0;
                    }
                }
                HOperatorSet.OverpaintRegion(ho_Image, ho_RegionToChange, hv_NewValue, "fill");
                ho_ImageOut.Dispose();
                ho_ImageOut = new HObject(ho_Image);
                ho_RegionToChange.Dispose();
                ho_RegionClass.Dispose();

                hv_IndexReset.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionToChange.Dispose();
                ho_RegionClass.Dispose();

                hv_IndexReset.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: File / Misc
        // Short Description: Remove a directory recursively. 
        public void remove_dir_recursively(HTuple hv_DirName)
        {



            // Local control variables 

            HTuple hv_Dirs = new HTuple(), hv_I = new HTuple();
            HTuple hv_Files = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //Recursively delete all subdirectories.
                hv_Dirs.Dispose();
                HOperatorSet.ListFiles(hv_DirName, "directories", out hv_Dirs);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Dirs.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        remove_dir_recursively(hv_Dirs.TupleSelect(hv_I));
                    }
                }
                //Delete all files.
                hv_Files.Dispose();
                HOperatorSet.ListFiles(hv_DirName, "files", out hv_Files);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Files.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.DeleteFile(hv_Files.TupleSelect(hv_I));
                    }
                }
                //Remove empty directory.
                HOperatorSet.RemoveDir(hv_DirName);

                hv_Dirs.Dispose();
                hv_I.Dispose();
                hv_Files.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Dirs.Dispose();
                hv_I.Dispose();
                hv_Files.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Remove invalid 3D pixels from a given domain. 
        private void remove_invalid_3d_pixels(HObject ho_ImageX, HObject ho_ImageY, HObject ho_ImageZ,
            HObject ho_Domain, out HObject ho_DomainOut, HTuple hv_InvalidPixelValue)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageXOut = null, ho_ImageYOut = null;
            HObject ho_ImageZOut = null, ho_RegionInvalX, ho_RegionInvalY;
            HObject ho_RegionInvalZ, ho_RegionInvalXY, ho_RegionInval;
            HObject ho_RegionInvalComplement;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainOut);
            HOperatorSet.GenEmptyObj(out ho_ImageXOut);
            HOperatorSet.GenEmptyObj(out ho_ImageYOut);
            HOperatorSet.GenEmptyObj(out ho_ImageZOut);
            HOperatorSet.GenEmptyObj(out ho_RegionInvalX);
            HOperatorSet.GenEmptyObj(out ho_RegionInvalY);
            HOperatorSet.GenEmptyObj(out ho_RegionInvalZ);
            HOperatorSet.GenEmptyObj(out ho_RegionInvalXY);
            HOperatorSet.GenEmptyObj(out ho_RegionInval);
            HOperatorSet.GenEmptyObj(out ho_RegionInvalComplement);
            try
            {
                ho_DomainOut.Dispose();
                ho_DomainOut = new HObject(ho_Domain);
                ho_ImageXOut.Dispose();
                ho_ImageXOut = new HObject(ho_ImageX);
                ho_ImageYOut.Dispose();
                ho_ImageYOut = new HObject(ho_ImageY);
                ho_ImageZOut.Dispose();
                ho_ImageZOut = new HObject(ho_ImageZ);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageXOut, ho_DomainOut, out ExpTmpOutVar_0);
                    ho_ImageXOut.Dispose();
                    ho_ImageXOut = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageYOut, ho_DomainOut, out ExpTmpOutVar_0);
                    ho_ImageYOut.Dispose();
                    ho_ImageYOut = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ReduceDomain(ho_ImageZOut, ho_DomainOut, out ExpTmpOutVar_0);
                    ho_ImageZOut.Dispose();
                    ho_ImageZOut = ExpTmpOutVar_0;
                }
                ho_RegionInvalX.Dispose();
                HOperatorSet.Threshold(ho_ImageXOut, out ho_RegionInvalX, hv_InvalidPixelValue,
                    hv_InvalidPixelValue);
                ho_RegionInvalY.Dispose();
                HOperatorSet.Threshold(ho_ImageYOut, out ho_RegionInvalY, hv_InvalidPixelValue,
                    hv_InvalidPixelValue);
                ho_RegionInvalZ.Dispose();
                HOperatorSet.Threshold(ho_ImageZOut, out ho_RegionInvalZ, hv_InvalidPixelValue,
                    hv_InvalidPixelValue);
                ho_RegionInvalXY.Dispose();
                HOperatorSet.Intersection(ho_RegionInvalX, ho_RegionInvalY, out ho_RegionInvalXY
                    );
                ho_RegionInval.Dispose();
                HOperatorSet.Intersection(ho_RegionInvalXY, ho_RegionInvalZ, out ho_RegionInval
                    );
                ho_RegionInvalComplement.Dispose();
                HOperatorSet.Complement(ho_RegionInval, out ho_RegionInvalComplement);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.Intersection(ho_DomainOut, ho_RegionInvalComplement, out ExpTmpOutVar_0
                        );
                    ho_DomainOut.Dispose();
                    ho_DomainOut = ExpTmpOutVar_0;
                }
                ho_ImageXOut.Dispose();
                ho_ImageYOut.Dispose();
                ho_ImageZOut.Dispose();
                ho_RegionInvalX.Dispose();
                ho_RegionInvalY.Dispose();
                ho_RegionInvalZ.Dispose();
                ho_RegionInvalXY.Dispose();
                ho_RegionInval.Dispose();
                ho_RegionInvalComplement.Dispose();


                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageXOut.Dispose();
                ho_ImageYOut.Dispose();
                ho_ImageZOut.Dispose();
                ho_RegionInvalX.Dispose();
                ho_RegionInvalY.Dispose();
                ho_RegionInvalZ.Dispose();
                ho_RegionInvalXY.Dispose();
                ho_RegionInval.Dispose();
                ho_RegionInvalComplement.Dispose();


                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Replace legacy preprocessing parameters or values. 
        private void replace_legacy_preprocessing_parameters(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Exception = new HTuple(), hv_NormalizationTypeExists = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_LegacyNormalizationKeyExists = new HTuple();
            HTuple hv_ContrastNormalization = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure adapts the dictionary DLPreprocessParam
                //if a legacy preprocessing parameter is set.
                //
                //Map legacy value set to new parameter.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_NormalizationTypeExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "normalization_type",
                        out hv_NormalizationTypeExists);
                    //
                    if ((int)(hv_NormalizationTypeExists) != 0)
                    {
                        hv_NormalizationType.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                        if ((int)(new HTuple(hv_NormalizationType.TupleEqual("true"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "first_channel";
                        }
                        else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("false"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "none";
                        }
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //
                //Map legacy parameter to new parameter and corresponding value.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_LegacyNormalizationKeyExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "contrast_normalization",
                        out hv_LegacyNormalizationKeyExists);
                    if ((int)(hv_LegacyNormalizationKeyExists) != 0)
                    {
                        hv_ContrastNormalization.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "contrast_normalization",
                            out hv_ContrastNormalization);
                        //Replace 'contrast_normalization' by 'normalization_type'.
                        if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual("false"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "none");
                        }
                        else if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual(
                            "true"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "first_channel");
                        }
                        HOperatorSet.RemoveDictKey(hv_DLPreprocessParam, "contrast_normalization");
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Text
        // Short Description: Set font independent of OS 
        public void set_display_font(HTuple hv_WindowHandle, HTuple hv_Size, HTuple hv_Font,
            HTuple hv_Bold, HTuple hv_Slant)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_OS = new HTuple(), hv_Fonts = new HTuple();
            HTuple hv_Style = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_AvailableFonts = new HTuple(), hv_Fdx = new HTuple();
            HTuple hv_Indices = new HTuple();
            HTuple hv_Font_COPY_INP_TMP = new HTuple(hv_Font);
            HTuple hv_Size_COPY_INP_TMP = new HTuple(hv_Size);

            // Initialize local and output iconic variables 
            try
            {
                //This procedure sets the text font of the current window with
                //the specified attributes.
                //
                //Input parameters:
                //WindowHandle: The graphics window for which the font will be set
                //Size: The font size. If Size=-1, the default of 16 is used.
                //Bold: If set to 'true', a bold font is used
                //Slant: If set to 'true', a slanted font is used
                //
                hv_OS.Dispose();
                HOperatorSet.GetSystem("operating_system", out hv_OS);
                if ((int)((new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                    new HTuple(hv_Size_COPY_INP_TMP.TupleEqual(-1)))) != 0)
                {
                    hv_Size_COPY_INP_TMP.Dispose();
                    hv_Size_COPY_INP_TMP = 16;
                }
                if ((int)(new HTuple(((hv_OS.TupleSubstr(0, 2))).TupleEqual("Win"))) != 0)
                {
                    //Restore previous behavior
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = ((1.13677 * hv_Size_COPY_INP_TMP)).TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                else
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Size = hv_Size_COPY_INP_TMP.TupleInt()
                                ;
                            hv_Size_COPY_INP_TMP.Dispose();
                            hv_Size_COPY_INP_TMP = ExpTmpLocalVar_Size;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("Courier"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Courier";
                    hv_Fonts[1] = "Courier 10 Pitch";
                    hv_Fonts[2] = "Courier New";
                    hv_Fonts[3] = "CourierNew";
                    hv_Fonts[4] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("mono"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Consolas";
                    hv_Fonts[1] = "Menlo";
                    hv_Fonts[2] = "Courier";
                    hv_Fonts[3] = "Courier 10 Pitch";
                    hv_Fonts[4] = "FreeMono";
                    hv_Fonts[5] = "Liberation Mono";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("sans"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Luxi Sans";
                    hv_Fonts[1] = "DejaVu Sans";
                    hv_Fonts[2] = "FreeSans";
                    hv_Fonts[3] = "Arial";
                    hv_Fonts[4] = "Liberation Sans";
                }
                else if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual("serif"))) != 0)
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple();
                    hv_Fonts[0] = "Times New Roman";
                    hv_Fonts[1] = "Luxi Serif";
                    hv_Fonts[2] = "DejaVu Serif";
                    hv_Fonts[3] = "FreeSerif";
                    hv_Fonts[4] = "Utopia";
                    hv_Fonts[5] = "Liberation Serif";
                }
                else
                {
                    hv_Fonts.Dispose();
                    hv_Fonts = new HTuple(hv_Font_COPY_INP_TMP);
                }
                hv_Style.Dispose();
                hv_Style = "";
                if ((int)(new HTuple(hv_Bold.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Bold";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Bold.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Bold";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Slant.TupleEqual("true"))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Style = hv_Style + "Italic";
                            hv_Style.Dispose();
                            hv_Style = ExpTmpLocalVar_Style;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Slant.TupleNotEqual("false"))) != 0)
                {
                    hv_Exception.Dispose();
                    hv_Exception = "Wrong value of control parameter Slant";
                    throw new HalconException(hv_Exception);
                }
                if ((int)(new HTuple(hv_Style.TupleEqual(""))) != 0)
                {
                    hv_Style.Dispose();
                    hv_Style = "Normal";
                }
                hv_AvailableFonts.Dispose();
                HOperatorSet.QueryFont(hv_WindowHandle, out hv_AvailableFonts);
                hv_Font_COPY_INP_TMP.Dispose();
                hv_Font_COPY_INP_TMP = "";
                for (hv_Fdx = 0; (int)hv_Fdx <= (int)((new HTuple(hv_Fonts.TupleLength())) - 1); hv_Fdx = (int)hv_Fdx + 1)
                {
                    hv_Indices.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Indices = hv_AvailableFonts.TupleFind(
                            hv_Fonts.TupleSelect(hv_Fdx));
                    }
                    if ((int)(new HTuple((new HTuple(hv_Indices.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        if ((int)(new HTuple(((hv_Indices.TupleSelect(0))).TupleGreaterEqual(0))) != 0)
                        {
                            hv_Font_COPY_INP_TMP.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Font_COPY_INP_TMP = hv_Fonts.TupleSelect(
                                    hv_Fdx);
                            }
                            break;
                        }
                    }
                }
                if ((int)(new HTuple(hv_Font_COPY_INP_TMP.TupleEqual(""))) != 0)
                {
                    throw new HalconException("Wrong value of control parameter Font");
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Font = (((hv_Font_COPY_INP_TMP + "-") + hv_Style) + "-") + hv_Size_COPY_INP_TMP;
                        hv_Font_COPY_INP_TMP.Dispose();
                        hv_Font_COPY_INP_TMP = ExpTmpLocalVar_Font;
                    }
                }
                HOperatorSet.SetFont(hv_WindowHandle, hv_Font_COPY_INP_TMP);

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Font_COPY_INP_TMP.Dispose();
                hv_Size_COPY_INP_TMP.Dispose();
                hv_OS.Dispose();
                hv_Fonts.Dispose();
                hv_Style.Dispose();
                hv_Exception.Dispose();
                hv_AvailableFonts.Dispose();
                hv_Fdx.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: OCR / Deep OCR
        // Short Description: Split rectangle2 into a number of rectangles. 
        private void split_rectangle2(HTuple hv_Row, HTuple hv_Column, HTuple hv_Phi,
            HTuple hv_Length1, HTuple hv_Length2, HTuple hv_NumSplits, out HTuple hv_SplitRow,
            out HTuple hv_SplitColumn, out HTuple hv_SplitPhi, out HTuple hv_SplitLength1Out,
            out HTuple hv_SplitLength2Out)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_SplitLength = new HTuple(), hv_TRow = new HTuple();
            HTuple hv_TCol = new HTuple(), hv_HomMat2D = new HTuple();
            // Initialize local and output iconic variables 
            hv_SplitRow = new HTuple();
            hv_SplitColumn = new HTuple();
            hv_SplitPhi = new HTuple();
            hv_SplitLength1Out = new HTuple();
            hv_SplitLength2Out = new HTuple();
            try
            {
                if ((int)(new HTuple(hv_NumSplits.TupleGreater(0))) != 0)
                {
                    hv_SplitLength.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SplitLength = hv_Length1 / (hv_NumSplits.TupleReal()
                            );
                    }
                    //Assume center (0,0), transform afterwards.
                    hv_TRow.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TRow = HTuple.TupleGenConst(
                            hv_NumSplits, 0.0);
                    }
                    hv_TCol.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_TCol = ((-hv_Length1) + hv_SplitLength) + ((HTuple.TupleGenSequence(
                            0, hv_NumSplits - 1, 1) * 2) * hv_SplitLength);
                    }
                    hv_HomMat2D.Dispose();
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_Phi, 0, 0, out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_Row, hv_Column, out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    hv_SplitLength1Out.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SplitLength1Out = HTuple.TupleGenConst(
                            hv_NumSplits, hv_SplitLength);
                    }
                    hv_SplitLength2Out.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SplitLength2Out = HTuple.TupleGenConst(
                            hv_NumSplits, hv_Length2);
                    }
                    hv_SplitPhi.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SplitPhi = HTuple.TupleGenConst(
                            hv_NumSplits, hv_Phi);
                    }
                    hv_SplitRow.Dispose(); hv_SplitColumn.Dispose();
                    HOperatorSet.AffineTransPoint2d(hv_HomMat2D, hv_TRow, hv_TCol, out hv_SplitRow,
                        out hv_SplitColumn);
                }
                else
                {
                    throw new HalconException("Number of splits must be greater than 0.");
                }

                hv_SplitLength.Dispose();
                hv_TRow.Dispose();
                hv_TCol.Dispose();
                hv_HomMat2D.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_SplitLength.Dispose();
                hv_TRow.Dispose();
                hv_TCol.Dispose();
                hv_HomMat2D.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Local procedures 
        public void clean_up_output(HTuple hv_OutputDir, HTuple hv_RemoveResults)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_WindowHandle = new HTuple(), hv_WarningCleanup = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //This local example procedure cleans up the output of the example.
                //
                if ((int)(hv_RemoveResults.TupleNot()) != 0)
                {

                    hv_WindowHandle.Dispose();
                    hv_WarningCleanup.Dispose();

                    return;
                }
                //Display a warning.
                HOperatorSet.SetWindowAttr("background_color", "black");
                HOperatorSet.OpenWindow(0, 0, 600, 300, 0, "visible", "", out hv_WindowHandle);
                HDevWindowStack.Push(hv_WindowHandle);
                set_display_font(hv_WindowHandle, 16, "mono", "true", "false");
                hv_WarningCleanup.Dispose();
                hv_WarningCleanup = new HTuple();
                hv_WarningCleanup[0] = "Congratulations, you have finished the example.";
                hv_WarningCleanup[1] = "";
                hv_WarningCleanup[2] = "Unless you would like to use the output data / model,";
                hv_WarningCleanup[3] = "press F5 to clean up.";
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_WarningCleanup, "window",
                        "center", "center", ((((new HTuple("black")).TupleConcat("black")).TupleConcat(
                        "coral")).TupleConcat("coral")).TupleConcat("coral"), new HTuple(), new HTuple());
                }
                //
                // stop(...); only in hdevelop
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.CloseWindow(HDevWindowStack.Pop());
                }
                //
                //Delete all outputs of the example.
                remove_dir_recursively(hv_OutputDir);
                HOperatorSet.DeleteFile("model_best.hdl");
                HOperatorSet.DeleteFile("model_best_info.hdict");

                hv_WindowHandle.Dispose();
                hv_WarningCleanup.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_WindowHandle.Dispose();
                hv_WarningCleanup.Dispose();

                throw HDevExpDefaultException;
            }
        }

        

        public void devide(HObject ho_Image, HTuple hv_DLPreprocessParam, HTuple hv_DLModelHandle,
            HTuple hv_ClassIDs, HTuple hv_ClassNames)
        {




            // Local iconic variables 

            HObject ho_ImageZoom, ho_ClassRegions, ho_ClassRegion = null;
            HObject ho_ConnectedRegions = null, ho_CurrentRegion = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_DLResult = new HTuple();
            HTuple hv_Areas = new HTuple(), hv_ClassIndex = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_ConnectIndex = new HTuple(), hv_Text = new HTuple();
            HTuple hv_BoxColor = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageZoom);
            HOperatorSet.GenEmptyObj(out ho_ClassRegions);
            HOperatorSet.GenEmptyObj(out ho_ClassRegion);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_CurrentRegion);
            try
            {
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                //生成为样本图片
                hv_DLSample.Dispose();
                gen_dl_samples_from_images(ho_Image, out hv_DLSample);
                //根据预处理的参数处理为何训练时候同一规格图片
                preprocess_dl_samples(hv_DLSample, hv_DLPreprocessParam);
                //将预处理的图片导入模型进行自推断，并输出结果DLResult，这个参数非常重要
                hv_DLResult.Dispose();
                HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSample, new HTuple(), out hv_DLResult);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_ImageZoom.Dispose();
                    HOperatorSet.ZoomImageSize(hv_DLResult.TupleGetDictObject("segmentation_image"),
                        out ho_ImageZoom, hv_Width, hv_Height, "constant");
                }
                ho_ClassRegions.Dispose();
                HOperatorSet.Threshold(ho_ImageZoom, out ho_ClassRegions, hv_ClassIDs, hv_ClassIDs);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.SetDraw(HDevWindowStack.GetActive(), "margin");
                }
                hv_Areas.Dispose();
                HOperatorSet.RegionFeatures(ho_ClassRegions, "area", out hv_Areas);
                if (HDevWindowStack.IsOpen())
                {
                    HOperatorSet.DispObj(ho_Image, HDevWindowStack.GetActive());
                }
                for (hv_ClassIndex = 1; (int)hv_ClassIndex <= (int)((new HTuple(hv_Areas.TupleLength()
                    )) - 1); hv_ClassIndex = (int)hv_ClassIndex + 1)
                {
                    if ((int)(new HTuple(((hv_Areas.TupleSelect(hv_ClassIndex))).TupleGreater(
                        50))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ClassRegion.Dispose();
                            HOperatorSet.SelectObj(ho_ClassRegions, out ho_ClassRegion, hv_ClassIndex + 1);
                        }
                        if (HDevWindowStack.IsOpen())
                        {
                            HOperatorSet.DispObj(ho_ClassRegion, HDevWindowStack.GetActive());
                        }
                        //Get connected components of the segmented class region.
                        ho_ConnectedRegions.Dispose();
                        HOperatorSet.Connection(ho_ClassRegion, out ho_ConnectedRegions);
                        hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                        HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                        for (hv_ConnectIndex = 0; (int)hv_ConnectIndex <= (int)((new HTuple(hv_Area.TupleLength()
                            )) - 1); hv_ConnectIndex = (int)hv_ConnectIndex + 1)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_CurrentRegion.Dispose();
                                HOperatorSet.SelectObj(ho_ConnectedRegions, out ho_CurrentRegion, hv_ConnectIndex + 1);
                            }
                            if (HDevWindowStack.IsOpen())
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    HOperatorSet.DispText(HDevWindowStack.GetActive(), (((hv_ClassNames.TupleSelect(
                                        hv_ClassIndex)) + "\narea: ") + (hv_Area.TupleSelect(hv_ConnectIndex))) + "px",
                                        "image", (hv_Row.TupleSelect(hv_ConnectIndex)) - 10, (hv_Column.TupleSelect(
                                        hv_ConnectIndex)) + 10, "black", new HTuple(), new HTuple());
                                }
                            }
                        }
                    }
                }
                if ((int)(new HTuple((((hv_Areas.TupleSum()) - (hv_Areas.TupleSelect(0)))).TupleGreater(
                    0))) != 0)
                {
                    hv_Text.Dispose();
                    hv_Text = "NOK";
                    hv_BoxColor.Dispose();
                    hv_BoxColor = "red";
                }
                else
                {
                    hv_Text.Dispose();
                    hv_Text = "OK";
                    hv_BoxColor.Dispose();
                    hv_BoxColor = "green";
                }
                if (HDevWindowStack.IsOpen())
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HOperatorSet.DispText(HDevWindowStack.GetActive(), hv_Text, "window", "top",
                            "left", "black", (new HTuple("box_color")).TupleConcat("shadow"), hv_BoxColor.TupleConcat(
                            "false"));
                    }
                }
                ho_ImageZoom.Dispose();
                ho_ClassRegions.Dispose();
                ho_ClassRegion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_CurrentRegion.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_DLSample.Dispose();
                hv_DLResult.Dispose();
                hv_Areas.Dispose();
                hv_ClassIndex.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_ConnectIndex.Dispose();
                hv_Text.Dispose();
                hv_BoxColor.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageZoom.Dispose();
                ho_ClassRegions.Dispose();
                ho_ClassRegion.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_CurrentRegion.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_DLSample.Dispose();
                hv_DLResult.Dispose();
                hv_Areas.Dispose();
                hv_ClassIndex.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_ConnectIndex.Dispose();
                hv_Text.Dispose();
                hv_BoxColor.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void init(HTuple hv_DLModelHandle, out HTuple hv_DLPreprocessParam, out HTuple hv_ClassNames,
            out HTuple hv_ClassIDs)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_DLDeviceHandles = new HTuple(), hv_DLDevice = new HTuple();
            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            hv_ClassNames = new HTuple();
            hv_ClassIDs = new HTuple();
            try
            {

                //从模型得到相应的预处理参数（你采用DTL训练时候的，如图片大小，通道等）
                hv_DLPreprocessParam.Dispose();
                create_dl_preprocess_param_from_model(hv_DLModelHandle, "none", "full_domain",
                    new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);
                //2 查询电脑硬件，如果查不到，抛出异常，终止后续执行
                hv_DLDeviceHandles.Dispose();
                HOperatorSet.QueryAvailableDlDevices((new HTuple("runtime")).TupleConcat("runtime"),
                    (new HTuple("gpu")).TupleConcat("cpu"), out hv_DLDeviceHandles);
                if ((int)(new HTuple((new HTuple(hv_DLDeviceHandles.TupleLength())).TupleEqual(
                    0))) != 0)
                {
                    throw new HalconException("No supported device found to continue.");
                }
                //默认将GPU方法赋值
                hv_DLDevice.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLDevice = hv_DLDeviceHandles.TupleSelect(
                        0);
                }
                //赋值给模型参数
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", 1);
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDevice);
                hv_ClassNames.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                hv_ClassIDs.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);

                hv_DLDeviceHandles.Dispose();
                hv_DLDevice.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_DLDeviceHandles.Dispose();
                hv_DLDevice.Dispose();

                throw HDevExpDefaultException;
            }
        }

    }
}
