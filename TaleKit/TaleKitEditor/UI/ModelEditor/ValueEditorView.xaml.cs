using GKitForWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TaleKit.Datas.ModelEditor;
using TaleKitEditor.UI.ModelEditor;

namespace TaleKitEditor.UI.ModelEditor {
	public delegate void ModelValueChangedDelegate(object model, FieldInfo field, IValueEditor valueEditorElement);

	[ContentProperty(nameof(Children))]
	public partial class ValueEditorView : UserControl {
		public static readonly DependencyPropertyKey ChildrenProperty = DependencyProperty.RegisterReadOnly(nameof(Children), typeof(UIElementCollection), typeof(ValueEditorView), new PropertyMetadata());
		public static readonly DependencyProperty ValueNameTextProperty = DependencyProperty.RegisterAttached(nameof(ValueNameText), typeof(string), typeof(ValueEditorView), new PropertyMetadata(null));

		private static Dictionary<Type, Type> ComponentAttr_To_ComponentViewDict = new Dictionary<Type, Type>() {
			{ typeof(ValueEditorComponent_FilePreviewAttribute), typeof(ValueEditorComponent_FilePreview) },
			{ typeof(ValueEditorComponent_HeaderAttribute), typeof(ValueEditorComponent_Header) },
			{ typeof(ValueEditorComponent_ItemSeparatorAttribute), typeof(ItemSeparator) },

		};
		private static Dictionary<Type, Type> EditorAttr_To_EditorViewDict = new Dictionary<Type, Type>() {
			{ typeof(ValueEditor_NumberBoxAttribute), typeof(ValueEditor_NumberBox) },
			{ typeof(ValueEditor_SliderAttribute), typeof(ValueEditor_Slider) },
			{ typeof(ValueEditor_SwitchAttribute), typeof(ValueEditor_Switch) },
			{ typeof(ValueEditor_TextBoxAttribute), typeof(ValueEditor_TextBox) },
			{ typeof(ValueEditor_Vector2Attribute), typeof(ValueEditor_Vector2) },
			{ typeof(ValueEditor_Vector3Attribute), typeof(ValueEditor_Vector3) },
			{ typeof(ValueEditor_ColorBoxAttribute), typeof(ValueEditor_ColorBox) },
			{ typeof(ValueEditor_AnchorPresetAttribute), typeof(ValueEditor_AnchorPreset) },
			{ typeof(ValueEditor_MarginAttribute), typeof(ValueEditor_Margin) },
			{ typeof(ValueEditor_AssetSelectorAttribute), typeof(ValueEditor_AssetSelector) }, 
			{ typeof(ValueEditor_TextBlockViewerAttribute), typeof(ValueEditor_TextBlockViewer) }, 
			{ typeof(ValueEditor_EnumComboBoxAttribute), typeof(ValueEditor_EnumComboBox) }, 
			{ typeof(ValueEditor_UiItemSelectorAttribute), typeof(ValueEditor_UiItemSelector) }, 
		};

		public event Action<object> ElementEditorValueChanged;

		public string ValueNameText {
			get {
				return (string)GetValue(ValueNameTextProperty);
			}
			set {
				SetValue(ValueNameTextProperty, value);
			}
		}
		public object EditableValue {
			get {
				return valueEditorElement.EditableValue;
			} set {
				valueEditorElement.EditableValue = value;
			}
		}
		protected IValueEditor valueEditorElement;

		protected EditableModel model;
		protected ValueEditorAttribute attribute;
		protected FieldInfo field;
		protected ModelEditorType editorType;

		public UIElementCollection Children {
			get { 
				return (UIElementCollection)GetValue(ChildrenProperty.DependencyProperty);
			} private set { 
				SetValue(ChildrenProperty, value);
			}
		}

		// [ Constructor ]
		public ValueEditorView() {
			InitializeComponent();

			Children = ValueEditorElementContext.Children;
		}
		public ValueEditorView(EditableModel model, FieldInfo field, ModelEditorType editorType) : this() {
			this.model = model;
			this.field = field;
			this.editorType = editorType;

			InitializeEditorType(editorType);

			BuildEditorContext();
		}
		private void InitializeEditorType(ModelEditorType editorType) {
			KeyFrameContext.Visibility = Visibility.Collapsed;

			if (editorType == ModelEditorType.EditKeyFrameModel) {
				KeyFrameContext.Visibility = Visibility.Visible;

				KeyFrameContext.RegisterButtonReaction();
				KeyFrameContext.RegisterClickEvent(OnKeyFrameMarkerClick, true);
			}
		}
		private void BuildEditorContext() {
			// Classify components : 헤더 등등 부가적인 정보
			ValueEditorComponentAttribute[] components = field.GetCustomAttributes(typeof(ValueEditorComponentAttribute)).Select(x => (ValueEditorComponentAttribute)x).ToArray();
			foreach (ValueEditorComponentAttribute component in components) {
				UserControl view = CreateEditorComponentView(component, model, field);

				ValueEditorComponentContext.Children.Add(view);
			}

			if (!IsAvailableValueEditorAttributeCount(field))
				return;

			// Set editor attributes
			attribute = field.GetCustomAttribute(typeof(ValueEditorAttribute)) as ValueEditorAttribute;
			ValueNameText = attribute.valueName;

			switch (attribute.layout) {
				case ValueEditorLayout.Wide:
					SetWideEditorElementContext();
					break;
			}

			// ValueEditor element
			UserControl editorElement = CreateEditorElementView(attribute, model, field);
			valueEditorElement = (IValueEditor)editorElement;

			valueEditorElement.EditableValueChanged += ElementValueChanged;
			valueEditorElement.EditableValue = field.GetValue(model);

			// Register events
			model.ModelUpdated += UpdateVisible;

			Children.Clear();
			Children.Add(editorElement);

			void ElementValueChanged(object value) {
				ElementEditorValueChanged?.Invoke(value);
				this.field.SetValue(model, Convert.ChangeType(value, this.field.FieldType));
				model.NotifyModelUpdated(model, field, valueEditorElement);
			}
		}

		// [ Event ]
		private void OnKeyFrameMarkerClick() {
			
		}

		// [ Create elements ]
		private UserControl CreateEditorComponentView(ValueEditorComponentAttribute attr, EditableModel model, FieldInfo fieldInfo) {
			UserControl view;
			if (attr is ValueEditorComponent_FilePreviewAttribute) {
				view = new ValueEditorComponent_FilePreview(fieldInfo.GetValue(model) as string);
			} else {
				view = CreateAttributeView(attr, ComponentAttr_To_ComponentViewDict);
			}
			return view;
		}
		private UserControl CreateEditorElementView(ValueEditorAttribute attr, EditableModel model, FieldInfo fieldInfo) {
			UserControl view;
			if(attr is ValueEditor_EnumComboBoxAttribute) {
				view = new ValueEditor_EnumComboBox(fieldInfo.FieldType);
			} else {
				view = CreateAttributeView(attr, EditorAttr_To_EditorViewDict);
			}
			return view;
		}

		private UserControl CreateAttributeView(Attribute attr, Dictionary<Type, Type> attrType_To_ViewTypeDict) {
			// Find constructor
			Type componentViewType = attrType_To_ViewTypeDict[attr.GetType()];
			ConstructorInfo[] componentViewConstructors = componentViewType.GetConstructors();
			ConstructorInfo componentViewConstructor = null;
			foreach (var constructorIterator in componentViewConstructors) {
				if (componentViewConstructor == null || constructorIterator.GetParameters().Length > componentViewConstructor.GetParameters().Length) {
					componentViewConstructor = constructorIterator;
				}
			}
			int constructorParamCount = componentViewConstructor.GetParameters().Length;

			// Call constructor
			UserControl view;
			if (constructorParamCount == 0) {
				view = (UserControl)componentViewConstructor.Invoke(null);
			} else if (constructorParamCount == 1) {
				view = (UserControl)componentViewConstructor.Invoke(new object[] { attr });
			} else {
				throw new Exception("Constructor parameter count is not in (0, 1).");
			}
			return view;
		}

		// [ Utility ]
		private void UpdateVisible(EditableModel model, FieldInfo fieldInfo, object editorView) {
			if (string.IsNullOrEmpty(attribute.visibleCondition))
				return;

			bool visible = (bool)model.GetType().GetProperty(attribute.visibleCondition, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(model);
			Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
		}

		private void SetWideEditorElementContext() {
			Grid.SetColumn(ValueEditorElementContext, 0);
			Grid.SetColumnSpan(ValueEditorElementContext, 2);
			Grid.SetRow(ValueEditorElementContext, 1);

			ValueSeparator.Visibility = Visibility.Visible;
		}

		private bool IsAvailableValueEditorAttributeCount(FieldInfo fieldInfo) {
			int editorAttrCount = fieldInfo.GetCustomAttributes<ValueEditorAttribute>().Count();
			if (editorAttrCount > 1) {
				throw new Exception("Field contains more than one ValueEditorAttribute.");
			}
			return editorAttrCount == 1;
		}
	}
}
