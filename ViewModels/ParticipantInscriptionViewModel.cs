using System.ComponentModel.DataAnnotations;

namespace AfroEvent.ViewModels
{
    public enum EvenementChoice
    {
        FeteEtudiant,
        FeteTabaski,
        Conference
    }

    public class ParticipantInscriptionViewModel
    {
        [Display(Name = "Nom complet")]
        [Required(ErrorMessage = "Le nom complet est obligatoire.")]
        [MinLength(3, ErrorMessage = "Le nom complet doit contenir au moins 3 caractères.")]
        public string NomComplet { get; set; } = string.Empty;

        [Display(Name = "Adresse e-mail")]
        [Required(ErrorMessage = "L'e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "Le format de l'e-mail est invalide.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Type de pass")]
        [Required(ErrorMessage = "Le type de pass est obligatoire.")]
        [EnumDataType(typeof(TypePass))]
        public TypePass TypeP { get; set; }

       

        [Display(Name = "Choisir un événement")]
        [Required(ErrorMessage = "Vous devez choisir un événement.")]
        [EnumDataType(typeof(EvenementChoice))]
        public EvenementChoice Evenement { get; set; }

        [Display(Name = "J'accepte les conditions")]
        [MustBeTrue(ErrorMessage = "Vous devez accepter les conditions.")]
        public bool AccepteConditions { get; set; }
    }
}
