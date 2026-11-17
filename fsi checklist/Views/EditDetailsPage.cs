// =====================================================================
// EDIT DETAILS PAGE: usbon ang unang gi-fill up sa inspection (ngalan, address, owner,
// contact number, IO no., petsa). Tanan kaugalingong controls (Controls/InspectionDetails).
// =====================================================================
using fsi_checklist.Controls.InspectionDetails;
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public sealed class EditDetailsPage : ContentPage
{
    private readonly EditDetailsViewModel _viewModel;

    public EditDetailsPage(EditDetailsViewModel viewModel)
    {
        BindingContext = _viewModel = viewModel;
        Title = "Edit: details";

        // STEP 1 sa Edit: detalye una (mao ang una nga gi-fill up), dayon ang mga tubag (entries)
        var step = new Label
        {
            Text = "Step 1 of 2: details. Next: the form entries.",
            FontSize = 12,
            TextColor = Colors.Gray,
        };
        var form = new Label { FontAttributes = FontAttributes.Bold, FontSize = 20 };
        form.SetBinding(Label.TextProperty, nameof(EditDetailsViewModel.FormName));

        var name = new DetailsTextField();
        name.SetBinding(DetailsTextField.TitleProperty, nameof(EditDetailsViewModel.SubjectLabel));
        name.SetBinding(DetailsTextField.TextProperty, nameof(EditDetailsViewModel.SubjectName), BindingMode.TwoWay);

        var address = new DetailsTextField { Title = "Address" };
        address.SetBinding(DetailsTextField.TextProperty, nameof(EditDetailsViewModel.Address), BindingMode.TwoWay);

        var owner = new DetailsTextField { Title = "Owner / representative" };
        owner.SetBinding(DetailsTextField.TextProperty, nameof(EditDetailsViewModel.OwnerName), BindingMode.TwoWay);

        var contact = new ContactNumberField();
        contact.SetBinding(ContactNumberField.TextProperty, nameof(EditDetailsViewModel.ContactNo), BindingMode.TwoWay);

        var reference = new DetailsTextField { Title = "Inspection order / control no." };
        reference.SetBinding(DetailsTextField.TextProperty, nameof(EditDetailsViewModel.ReferenceNo), BindingMode.TwoWay);

        var date = new InspectedDateField();
        date.SetBinding(InspectedDateField.DateProperty, nameof(EditDetailsViewModel.DateInspected), BindingMode.TwoWay);

        var save = new ContinueToEntriesButton();
        save.SetBinding(Button.CommandProperty, nameof(EditDetailsViewModel.SaveCommand));

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 16,
                Spacing = 12,
                Children = { form, step, name, address, owner, contact, reference, date, save },
            },
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
