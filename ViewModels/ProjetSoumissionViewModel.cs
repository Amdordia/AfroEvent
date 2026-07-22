using System.ComponentModel.DataAnnotations;

namespace AfroEvent.ViewModels // On utilise le namespace de notre projet !
{
    public class ProjetSoumissionViewModel
    {
        [Display(Name = "Titre du projet")]
        [Required(ErrorMessage = "Le titre du projet est obligatoire.")]
        [MinLength(5, ErrorMessage = "Le titre doit contenir au moins 5 caractères.")]
        public string Titre { get; set; } = string.Empty;

        [Display(Name = "Description détaillée")]
        // TODO 1 
        [Required(ErrorMessage = "La description est requise pour comprendre votre projet.")] 
        // TODO 2 
        [DataType(DataType.MultilineText)] 
        public string Description { get; set; } = string.Empty;

        [Display(Name = "Nom du chef d'équipe")]
        [Required(ErrorMessage = "Le nom du chef d'équipe est requis.")]
        public string ChefEquipe { get; set; } = string.Empty;

        [Display(Name = "Adresse e-mail de contact")]
        [Required(ErrorMessage = "L'e-mail est obligatoire.")]
        // TODO 3 
        [EmailAddress(ErrorMessage = "Le format de l'e-mail est invalide.")] 
        public string EmailContact { get; set; } = string.Empty;

        [Display(Name = "Budget estimé (€)")]
        [Range(100, 5000, ErrorMessage = "Le budget doit être compris entre 100€ et 5000€.")]
        public decimal BudgetEstime { get; set; }
    }
}