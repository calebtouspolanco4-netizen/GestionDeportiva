using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GestionDeportiva.Models;

namespace GestionDeportiva.ViewModels
{
    public class ReservasIndexViewModel
    {
        public List<ReservaItemViewModel> Reservas { get; set; } = new();
        public List<Escenario> Escenarios { get; set; } = new();
        public List<Cliente> Clientes { get; set; } = new();
        public List<EstadosReserva> Estados { get; set; } = new();
        public List<MetodosPago> MetodosPago { get; set; } = new();

        // Filtros
        public string? Busqueda { get; set; }
        public int? EscenarioId { get; set; }
        public int? EstadoId { get; set; }
        public string? Fecha { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; } = 1;
        public int PorPagina { get; set; } = 10;

        // Métricas
        public int TotalReservas { get; set; }
        public int ReservasConfirmadas { get; set; }
        public int ReservasPendientes { get; set; }
        public int ReservasCanceladas { get; set; }
        public int ReservasCompletadas { get; set; }
    }

    public class ReservaItemViewModel
    {
        public int ReservaId { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string ClienteTelefono { get; set; } = "";
        public bool TieneWhatsapp { get; set; }

        public int EscenarioId { get; set; }
        public string EscenarioNombre { get; set; } = "";
        public string DeporteNombre { get; set; } = "";

        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public double DuracionHoras { get; set; }

        public decimal MontoTotal { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente => Math.Max(0, MontoTotal - MontoPagado);
        public string EstadoPago => SaldoPendiente <= 0 ? "Pagado" : (MontoPagado > 0 ? "Abono parcial" : "Pendiente");

        public int EstadoId { get; set; }
        public string EstadoNombre { get; set; } = "";
        public string? Notas { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }

    public class CrearReservaInputModel
    {
        [Required(ErrorMessage = "Debe seleccionar un cliente.")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un escenario.")]
        public int EscenarioId { get; set; }

        [Required(ErrorMessage = "La fecha de reserva es requerida.")]
        public DateOnly FechaReserva { get; set; }

        [Required(ErrorMessage = "La hora de inicio es requerida.")]
        public TimeOnly HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es requerida.")]
        public TimeOnly HoraFin { get; set; }

        public decimal MontoTotal { get; set; }

        public string? Notas { get; set; }

        // Abono o pago inicial opcional
        public bool RegistrarPagoInicial { get; set; }
        public decimal? MontoAbono { get; set; }
        public int? MetodoPagoId { get; set; }
        public string? ReferenciaPago { get; set; }
    }
}

