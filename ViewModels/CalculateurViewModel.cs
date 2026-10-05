namespace CalculateurAge.ViewModels;

// Contient l'ÉTAT de l'écran et les ACTIONS possibles.
public class CalculateurViewModel : BaseViewModel
{
    // Champs privés : la vraie donnée.
    private string _nom = "";
    private DateTime _dateNaissance = DateTime.Today.AddYears(-20);
    private string _resultat = "";
    private string _message = "";
    private string _anniversaire = "";
    private bool _resultatVisible;

    // Propriétés publiques : ce que le XAML voit.
    public string Nom
    {
        get => _nom;
        set { if (SetField(ref _nom, value)) CalculerCommand.Rafraichir(); }
    }

    public DateTime DateNaissance
    {
        get => _dateNaissance;
        set => SetField(ref _dateNaissance, value);
    }

    public string Resultat
    {
        get => _resultat;
        set => SetField(ref _resultat, value);
    }

    // Fonctionnalité 1 : « Majeur » ou « Mineur ».
    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    // Fonctionnalité 3 : jours avant le prochain anniversaire.
    public string Anniversaire
    {
        get => _anniversaire;
        set => SetField(ref _anniversaire, value);
    }

    public bool ResultatVisible
    {
        get => _resultatVisible;
        set => SetField(ref _resultatVisible, value);
    }

    // Lié à Button.Command dans le XAML.
    public RelayCommand CalculerCommand { get; }

    // Fonctionnalité 2 : remise à zéro de l'écran.
    public RelayCommand EffacerCommand { get; }

    public CalculateurViewModel()
    {
        CalculerCommand = new RelayCommand(
            Calculer,
            () => !string.IsNullOrWhiteSpace(Nom));

        EffacerCommand = new RelayCommand(Effacer);
    }

    // La logique métier : aucun contrôle d'interface ici.
    private void Calculer()
    {
        int age = DateTime.Today.Year - DateNaissance.Year;
        if (DateNaissance.Date > DateTime.Today.AddYears(-age)) age--;

        Resultat = $"{Nom}, vous avez {age} ans";

        if (age < 0)
        {
            Message = "Date de naissance dans le futur";
            Anniversaire = "";
        }
        else
        {
            Message = age >= 18 ? "Majeur" : "Mineur";

            int jours = JoursAvantAnniversaire(DateNaissance);
            Anniversaire = jours switch
            {
                0 => "Joyeux anniversaire ! C'est aujourd'hui !",
                1 => "Prochain anniversaire : demain",
                _ => $"Prochain anniversaire dans {jours} jours"
            };
        }

        ResultatVisible = true;
    }

    // Remet tous les champs à leur valeur initiale.
    private void Effacer()
    {
        Nom = "";
        DateNaissance = DateTime.Today.AddYears(-20);
        Resultat = "";
        Message = "";
        Anniversaire = "";
        ResultatVisible = false;
    }

    // Nombre de jours entre aujourd'hui et le prochain anniversaire.
    private static int JoursAvantAnniversaire(DateTime naissance)
    {
        DateTime aujourdhui = DateTime.Today;
        DateTime prochain = AnniversaireEn(naissance, aujourdhui.Year);
        if (prochain < aujourdhui)
            prochain = AnniversaireEn(naissance, aujourdhui.Year + 1);
        return (prochain - aujourdhui).Days;
    }

    // Date de l'anniversaire pour une année donnée.
    // Le 29 février devient le 28 les années non bissextiles.
    private static DateTime AnniversaireEn(DateTime naissance, int annee)
    {
        int jour = naissance.Month == 2 && naissance.Day == 29
                   && !DateTime.IsLeapYear(annee)
            ? 28
            : naissance.Day;
        return new DateTime(annee, naissance.Month, jour);
    }
}