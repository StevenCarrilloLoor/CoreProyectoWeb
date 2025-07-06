import React, { useState, useEffect } from 'react';
import { LineChart, Line, BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { AlertCircle, TrendingUp, Building, Calendar, CheckCircle, XCircle, Clock, Search, RefreshCw } from 'lucide-react';

const API_BASE_URL = 'http://localhost:5073/api';

const App = () => {
  const [activeTab, setActiveTab] = useState('dashboard');
  const [dashboardData, setDashboardData] = useState(null);
  const [alertas, setAlertas] = useState([]);
  const [estaciones, setEstaciones] = useState([]);
  const [ventasPorDia, setVentasPorDia] = useState([]);
  const [topEstaciones, setTopEstaciones] = useState([]);
  const [alertasPorEstacion, setAlertasPorEstacion] = useState([]);
  const [loading, setLoading] = useState(true);
  const [selectedDate, setSelectedDate] = useState(new Date().toISOString().split('T')[0]);

  // Cargar datos del dashboard
  useEffect(() => {
    if (activeTab === 'dashboard') {
      cargarDashboard();
    } else if (activeTab === 'alertas') {
      cargarAlertas();
    } else if (activeTab === 'estaciones') {
      cargarEstaciones();
    }
  }, [activeTab]);

  const cargarDashboard = async () => {
    setLoading(true);
    try {
      const [resumen, ventas, top, alertasRecientes, alertasEstacion] = await Promise.all([
        fetch(`${API_BASE_URL}/DashboardApi/resumen`).then(r => {
          if (!r.ok) throw new Error('Error al cargar resumen');
          return r.json();
        }),
        fetch(`${API_BASE_URL}/DashboardApi/ventas-por-dia?dias=30`).then(r => {
          if (!r.ok) throw new Error('Error al cargar ventas');
          return r.json();
        }),
        fetch(`${API_BASE_URL}/DashboardApi/top-estaciones`).then(r => {
          if (!r.ok) throw new Error('Error al cargar top estaciones');
          return r.json();
        }),
        fetch(`${API_BASE_URL}/DashboardApi/alertas-recientes`).then(r => {
          if (!r.ok) throw new Error('Error al cargar alertas recientes');
          return r.json();
        }),
        fetch(`${API_BASE_URL}/DashboardApi/alertas-por-estacion`).then(r => {
          if (!r.ok) throw new Error('Error al cargar alertas por estación');
          return r.json();
        })
      ]);

      setDashboardData(resumen);
      setVentasPorDia(ventas.map(v => ({
        ...v,
        fecha: new Date(v.fecha).toLocaleDateString('es-ES', { day: '2-digit', month: 'short' })
      })));
      setTopEstaciones(top);
      setAlertas(alertasRecientes);
      setAlertasPorEstacion(alertasEstacion);
    } catch (error) {
      console.error('Error cargando dashboard:', error);
      alert('Error al conectar con el servidor. Verifique que el backend esté ejecutándose en http://localhost:5073');
    }
    setLoading(false);
  };

  const cargarAlertas = async (filtros = {}) => {
    setLoading(true);
    try {
      const queryParams = new URLSearchParams(filtros).toString();
      const response = await fetch(`${API_BASE_URL}/AlertasApi?${queryParams}`);
      const data = await response.json();
      setAlertas(data);
    } catch (error) {
      console.error('Error cargando alertas:', error);
    }
    setLoading(false);
  };

  const cargarEstaciones = async () => {
    setLoading(true);
    try {
      const response = await fetch(`${API_BASE_URL}/EstacionesApi`);
      const data = await response.json();
      setEstaciones(data);
    } catch (error) {
      console.error('Error cargando estaciones:', error);
    }
    setLoading(false);
  };

  const analizarFecha = async () => {
    try {
      const response = await fetch(`${API_BASE_URL}/AlertasApi/analizar`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ fecha: selectedDate })
      });
      const result = await response.json();
      alert(result.mensaje);
      if (result.alertasCreadas > 0) {
        cargarAlertas();
      }
    } catch (error) {
      console.error('Error analizando fecha:', error);
      alert('Error al analizar la fecha');
    }
  };

  const resolverAlerta = async (alertaId, estado) => {
    try {
      const response = await fetch(`${API_BASE_URL}/AlertasApi/${alertaId}/resolver`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ estado })
      });
      const result = await response.json();
      alert(result.mensaje);
      cargarAlertas();
    } catch (error) {
      console.error('Error resolviendo alerta:', error);
      alert('Error al resolver la alerta');
    }
  };

  const COLORS = ['#3B82F6', '#10B981', '#F59E0B', '#EF4444'];

  const getEstadoIcon = (estado) => {
    switch (estado) {
      case 'Pendiente':
        return <Clock className="w-4 h-4 text-yellow-500" />;
      case 'Confirmado':
        return <XCircle className="w-4 h-4 text-red-500" />;
      case 'Falso Positivo':
        return <CheckCircle className="w-4 h-4 text-green-500" />;
      default:
        return null;
    }
  };

  const renderDashboard = () => {
    if (!dashboardData) return <div className="text-center p-8">No hay datos disponibles. Asegúrese de que el backend esté ejecutándose.</div>;

    return (
      <div className="space-y-6">
        {/* Tarjetas de métricas */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Estaciones Activas</p>
                <p className="text-2xl font-bold">{dashboardData.general?.totalEstaciones || 0}</p>
              </div>
              <Building className="w-8 h-8 text-blue-500" />
            </div>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Alertas Pendientes</p>
                <p className="text-2xl font-bold">{dashboardData.general?.alertasPendientes || 0}</p>
                <p className="text-xs text-gray-500">De {dashboardData.general?.totalAlertas || 0} totales</p>
              </div>
              <AlertCircle className="w-8 h-8 text-yellow-500" />
            </div>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Ventas (Últimos 30 días)</p>
                <p className="text-2xl font-bold">${(dashboardData.ventasMes?.montoTotal || 0).toFixed(2)}</p>
                <p className="text-xs text-gray-500">{dashboardData.ventasMes?.cantidad || 0} transacciones</p>
              </div>
              <TrendingUp className="w-8 h-8 text-green-500" />
            </div>
          </div>
          <div className="bg-white p-6 rounded-lg shadow">
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm text-gray-600">Litros (Últimos 30 días)</p>
                <p className="text-2xl font-bold">{(dashboardData.ventasMes?.litrosVendidos || 0).toFixed(2)}</p>
              </div>
              <Calendar className="w-8 h-8 text-purple-500" />
            </div>
          </div>
        </div>

        {/* Gráficos */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div className="bg-white p-6 rounded-lg shadow">
            <h3 className="text-lg font-semibold mb-4">Ventas por Día</h3>
            <ResponsiveContainer width="100%" height={300}>
              <LineChart data={ventasPorDia}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="fecha" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="montoTotal" stroke="#3B82F6" name="Monto Total" />
                <Line type="monotone" dataKey="litrosTotal" stroke="#10B981" name="Litros Vendidos" />
              </LineChart>
            </ResponsiveContainer>
          </div>

          <div className="bg-white p-6 rounded-lg shadow">
            <h3 className="text-lg font-semibold mb-4">Top 5 Estaciones del Mes</h3>
            <ResponsiveContainer width="100%" height={300}>
              <BarChart data={topEstaciones}>
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="estacionNombre" />
                <YAxis />
                <Tooltip />
                <Legend />
                <Bar dataKey="montoTotal" fill="#3B82F6" name="Monto Total" />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Alertas Recientes */}
        <div className="bg-white p-6 rounded-lg shadow">
          <h3 className="text-lg font-semibold mb-4">Alertas Recientes</h3>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead>
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estación</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estado</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {alertas.slice(0, 5).map((alerta) => (
                  <tr key={alerta.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{alerta.tipo}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{alerta.estacion || 'N/A'}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {new Date(alerta.fechaDeteccion).toLocaleDateString('es-ES')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="flex items-center">
                        {getEstadoIcon(alerta.estado)}
                        <span className="ml-2 text-sm">{alerta.estado}</span>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {/* Nueva sección: Alertas por Estación */}
        <div className="bg-white p-6 rounded-lg shadow">
          <h3 className="text-lg font-semibold mb-4">Estaciones con Más Alertas</h3>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead>
                <tr>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estación</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Total Alertas</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Pendientes</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Confirmadas</th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Falsos Positivos</th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {alertasPorEstacion.slice(0, 5).map((est, index) => (
                  <tr key={index} className={index === 0 ? 'bg-red-50' : ''}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">{est.estacion}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                      <span className={`font-bold ${index === 0 ? 'text-red-600' : ''}`}>{est.totalAlertas}</span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{est.pendientes}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{est.confirmadas}</td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">{est.falsosPositivos}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    );
  };

  const renderAlertas = () => {
    return (
      <div className="space-y-6">
        {/* Barra de herramientas */}
        <div className="bg-white p-4 rounded-lg shadow flex flex-col md:flex-row gap-4">
          <div className="flex-1 flex gap-2">
            <input
              type="date"
              value={selectedDate}
              onChange={(e) => setSelectedDate(e.target.value)}
              className="px-4 py-2 border rounded-lg"
            />
            <button
              onClick={analizarFecha}
              className="px-4 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-600 flex items-center gap-2"
            >
              <Search className="w-4 h-4" />
              Analizar Fecha
            </button>
          </div>
          <button
            onClick={() => cargarAlertas()}
            className="px-4 py-2 bg-gray-100 text-gray-700 rounded-lg hover:bg-gray-200 flex items-center gap-2"
          >
            <RefreshCw className="w-4 h-4" />
            Actualizar
          </button>
        </div>

        {/* Lista de alertas */}
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">ID</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Tipo</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Descripción</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estación</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Fecha</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Estado</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Acciones</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {alertas.map((alerta) => (
                <tr key={alerta.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">#{alerta.id}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{alerta.tipo}</td>
                  <td className="px-6 py-4 text-sm text-gray-500">
                    <div className="max-w-xs overflow-hidden text-ellipsis">{alerta.descripcion}</div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {alerta.estacion || 'N/A'}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(alerta.fechaDeteccion).toLocaleDateString('es-ES')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center">
                      {getEstadoIcon(alerta.estado)}
                      <span className="ml-2 text-sm">{alerta.estado}</span>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                    {alerta.estado === 'Pendiente' && (
                      <div className="flex gap-2">
                        <button
                          onClick={() => resolverAlerta(alerta.id, 'Confirmado')}
                          className="text-red-600 hover:text-red-900"
                        >
                          Confirmar
                        </button>
                        <button
                          onClick={() => resolverAlerta(alerta.id, 'Falso Positivo')}
                          className="text-green-600 hover:text-green-900"
                        >
                          Falso Positivo
                        </button>
                      </div>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    );
  };

  const renderEstaciones = () => {
    return (
      <div className="space-y-6">
        <div className="bg-white p-4 rounded-lg shadow">
          <h3 className="text-lg font-semibold mb-4">Estaciones de Servicio</h3>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {estaciones.map((estacion) => (
              <div key={estacion.id} className="border rounded-lg p-4 hover:shadow-lg transition-shadow">
                <h4 className="font-semibold text-lg">{estacion.nombre}</h4>
                <p className="text-sm text-gray-600 mt-1">{estacion.ubicacion}</p>
                <div className="mt-3 flex justify-between items-center">
                  <span className="text-xs text-gray-500">Código: {estacion.codigo}</span>
                  <span className={`px-2 py-1 text-xs rounded-full ${
                    estacion.activo ? 'bg-green-100 text-green-800' : 'bg-red-100 text-red-800'
                  }`}>
                    {estacion.activo ? 'Activo' : 'Inactivo'}
                  </span>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  };

  return (
    <div className="min-h-screen bg-gray-100">
      {/* Header */}
      <header className="bg-white shadow">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between items-center py-6">
            <h1 className="text-3xl font-bold text-gray-900">
              Sistema de Detección de Fraude - Petrolrios
            </h1>
          </div>
        </div>
      </header>

      {/* Navigation */}
      <nav className="bg-white border-b">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex space-x-8">
            <button
              onClick={() => setActiveTab('dashboard')}
              className={`py-4 px-1 border-b-2 font-medium text-sm ${
                activeTab === 'dashboard'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              Dashboard
            </button>
            <button
              onClick={() => setActiveTab('alertas')}
              className={`py-4 px-1 border-b-2 font-medium text-sm ${
                activeTab === 'alertas'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              Alertas
            </button>
            <button
              onClick={() => setActiveTab('estaciones')}
              className={`py-4 px-1 border-b-2 font-medium text-sm ${
                activeTab === 'estaciones'
                  ? 'border-blue-500 text-blue-600'
                  : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
              }`}
            >
              Estaciones
            </button>
          </div>
        </div>
      </nav>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        {loading ? (
          <div className="flex justify-center items-center h-64">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-500"></div>
          </div>
        ) : (
          <>
            {activeTab === 'dashboard' && renderDashboard()}
            {activeTab === 'alertas' && renderAlertas()}
            {activeTab === 'estaciones' && renderEstaciones()}
          </>
        )}
      </main>
    </div>
  );
};

export default App;