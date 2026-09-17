using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GestionDeportiva.Models;

namespace GestionDeportiva.ViewModels
{
    public class PagosIndexViewModel
    {
        public List<PagoItemViewModel> Pagos { get; set; } = new();
        public List<MetodosPago> MetodosPago { get; set; } = new();
        public List<ReservaPendienteViewModel> ReservasPendientes { get; set; } = new();

        // Filtros
        public string? Busqueda { get; set; }
        public int? MetodoPagoId { get; set; }
        public string? FechaDesde { get; set; }
        public string? FechaHasta { get; set; }
        public int PaginaActual { get; set; } = 1;
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; } = 1;
        public int PorPagina { get; set; } = 10;

        // Modal registrar abierto
        public bool AbrirModalRegistrar { get; set; }
        public int? PreseleccionarReservaId { get; set; }

        // Métricas
        public decimal TotalRecaudadoMes { get; set; }
        public decimal TotalRecaudadoHoy { get; set; }
        public int TotalTransaccionesMes { get; set; }
        public decimal SaldoTotalPendiente { get; set; }
    }

    public class PagoItemViewModel
    {
        public int PagoId { get; set; }
        public int ReservaId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public int MetodoPagoId { get; set; }
        public string MetodoPagoNombre { get; set; } = "";
        public string? ReferenciaTransaccion { get; set; }

        public string ClienteNombre { get; set; } = "";
        public string ClienteTelefono { get; set; } = "";
        public string EscenarioNombre { get; set; } = "";
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public decimal MontoTotalReserva { get; set; }
    }

    public class ReservaPendienteViewModel
    {
        public int ReservaId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string EscenarioNombre { get; set; } = "";
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoPagado { get; set; }
        public decimal SaldoPendiente { get; set; }
    }

    public class RegistrarPagoInputModel
    {
        [Required(ErrorMessage = "Debe seleccionar una reserva.")]
        public int ReservaId { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un método de pago.")]
        public int MetodoPagoId { get; set; }

        [Required(ErrorMessage = "El monto es obligatorio.")]
        [Range(1, 100000000, ErrorMessage = "El monto debe ser mayor a cero.")]
        public decimal Monto { get; set; }

        public string? ReferenciaTransaccion { get; set; }
    }

    public class ComprobantePagoViewModel
    {
        public int PagoId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal MontoPagado { get; set; }
        public string MetodoPago { get; set; } = "";
        public string? ReferenciaTransaccion { get; set; }

        public int ReservaId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string ClienteTelefono { get; set; } = "";
        public string EscenarioNombre { get; set; } = "";
        public DateOnly FechaReserva { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }

        public decimal MontoTotalReserva { get; set; }
        public decimal TotalPagadoReserva { get; set; }
        public decimal SaldoRestante => Math.Max(0, MontoTotalReserva - TotalPagadoReserva);
        public string EstablecimientoNombre { get; set; } = "La Jugada - Canchas Sintéticas";
        public string EstablecimientoDireccion { get; set; } = "";
        public string EstablecimientoTelefono { get; set; } = "";
    }
}

