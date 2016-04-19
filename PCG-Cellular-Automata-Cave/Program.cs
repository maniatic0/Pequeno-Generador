using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace PCG_Cellular_Automata_Cave
{
    public enum tipo {
        vacio,
        pared
    }

    class Program
    {
        static tipo[][] mapa;
        static int columnas;
        static int filas;
        static int cantParedes = 0;
        static int cantIter;
        static int porcentajeInicial;
        static bool mostrar = false;
        static bool mostrarContinuo = true;
        static Random generador = new Random();
        static ConsoleKey c = ConsoleKey.Escape;

        static void Main(string[] args)
        {
            string linea;
            tipo[][] mapaviejo;
            float porcentajeCeldas;
            int iterMax, porcentajeEsperado, porcentajeMaximo, cantMinimaPared, radioPared, cantMaximaVacio, radioVacio, totalCeldas, radioExplosion, cantidadExplosiones, iterHechas = 0;
            do
            {
                Console.Clear();
                linea = Preguntar("Mostrar Steps: ");
                mostrar = ConvertiraBool(linea);
                if (mostrar)
                {
                    linea = Preguntar("Mostrar Continuo: ");
                    mostrarContinuo = ConvertiraBool(linea);
                }
                linea = Preguntar("Columnas: ");
                columnas = int.Parse(linea);
                linea = Preguntar("Filas: ");
                filas = int.Parse(linea);
                linea = Preguntar("Iteraciones Iniciales(Recomendado 10): ");
                cantIter = int.Parse(linea);
                linea = Preguntar("Maxima cantidad de Iteraciones: ");
                iterMax = int.Parse(linea);
                linea = Preguntar("Porcentaje Inicial de Paredes [0..100]: ");
                porcentajeInicial = int.Parse(linea);
                linea = Preguntar("Porcentaje Minimo de Paredes [0..100]: ");
                porcentajeEsperado = int.Parse(linea);
                linea = Preguntar("Porcentaje Maximo de Paredes [0..100]: ");
                porcentajeMaximo = int.Parse(linea);
                Console.WriteLine("Regla 4: El punto p si ya es pared seguira siendolo si tiene X vecinos pared en un radio r. Si p no es pared entonces p sera pared si tiene X+1 vecinos en un radio r.");
                Console.WriteLine("Recomendado X=4 y r=1");
                linea = Preguntar("X: ");
                cantMinimaPared = int.Parse(linea);
                linea = Preguntar("r: ");
                radioPared = int.Parse(linea);
                Console.WriteLine("Regla 5: Esta regla solo se activara si hay menos paredes del porcentaje esperado. Si p no es pared entonces p sera pared si tiene menos de Y vecinos pared en un radio r2 .");
                Console.WriteLine("Recomendado Y=2 y r2=2");
                linea = Preguntar("Y: ");
                cantMaximaVacio = int.Parse(linea);
                linea = Preguntar("r2: ");
                radioVacio = int.Parse(linea);
                Console.WriteLine("Regla 6: Esta regla solo se activara si hay mas paredes que el porcentaje maximo. Explotaran secciones aleatoreas del mapa con un radio que es un porcentaje de la cantidad total de celdas.");
                linea = Preguntar("Porcentaje Regla 6 [0.0..100.0] (Recomendado menos de 10% y mayor que 2%): ");
                porcentajeCeldas = float.Parse(linea);
                totalCeldas = filas * columnas;
                radioExplosion = Convert.ToInt32(Math.Sqrt((float)totalCeldas * porcentajeCeldas / 100.0));
                cantidadExplosiones = Convert.ToInt32((float)totalCeldas / (float)(radioExplosion * radioExplosion));

                do
                {
                    Console.Clear();
                    mapa = InicializarMapa(columnas, filas, porcentajeInicial);
                    Console.WriteLine("Original:\n");
                    Mostrar();
                    Esperar("Press enter to continue...");
                    for (int i = 0; i < cantIter && i < iterMax + 2; i++)
                    {
                        if ((double)cantParedes / (double)totalCeldas < (double)porcentajeEsperado / 100.0 && i < iterMax + 1)
                        {
                            mapa = AplicarRegla4(mapa, cantMinimaPared, cantMaximaVacio, radioPared, radioVacio);
                            cantIter += 1;
                        }
                        else if ((double)cantParedes / (double)totalCeldas >= (double)porcentajeMaximo / 100.0 && i < iterMax)
                        {
                            Explosiones(generador.Next(1, cantidadExplosiones), radioExplosion);
                            cantIter += 1;
                        }
                        else
                        {
                            mapaviejo = mapa;
                            mapa = AplicarRegla4(mapa, cantMinimaPared, -1, radioPared, radioVacio);
                            if (MapasIguales(mapaviejo))
                            {
                                break;
                            }
                        }

                        Console.Write("Iteracion ");
                        Console.Write(i);

                        if (mostrar)
                        {
                            if (mostrarContinuo)
                            {
                                Console.Clear();
                                Console.Write("Iteracion ");
                                Console.Write(i);
                                Console.WriteLine(" :\n");
                                Mostrar();
                                Thread.Sleep(40);
                            }
                            else
                            {
                                Console.WriteLine(" :");
                                Mostrar();
                                Esperar("Press enter to continue...");
                            }
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                        iterHechas = i;
                    }
                    Console.Clear();
                    Console.Write("Final luego de ");
                    Console.Write(iterHechas);
                    Console.WriteLine(" iteraciones: \n");
                    Mostrar();
                    Console.WriteLine("Press enter to continue or Any other key to exit to menu...");
                    c = Console.ReadKey().Key;
                } while (c == ConsoleKey.Enter);
                Console.WriteLine("Press enter to continue or Any other key to exit...");
                c = Console.ReadKey().Key;
            } while (c == ConsoleKey.Enter);
        }

        static bool ConvertiraBool(string entrada)
        {
            if (entrada.ToLower() == "t" || entrada.ToLower() == "true" || (entrada != "0" && entrada.All(char.IsDigit) && entrada != "") || entrada.ToLower() == "y" 
                || entrada.ToLower() == "yes" || entrada.ToLower() == "s" || entrada.ToLower() == "si")
            {
                return true;
            }
            return false;
        }

        static string Preguntar(string prompt = "")
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        static void Esperar(string prompt = "") {
            Console.Write(prompt);
            Console.ReadLine();
        }

        static bool PosValida(int x, int y)
        {
            return 0 <= x && x < columnas && 0 <= y && y < filas;
        }

        static int CantidadVecinosPared(int x, int y, int scopeX, int scopeY)
        {
            int startX = x - scopeX;
            int startY = y - scopeY;
            int endX = x + scopeX + 1;
            int endY = y + scopeY + 1;

            int cantidad = 0;

            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    if (PosValida(i,j) && (x != i || y != j))
                    {
                        cantidad += (int)mapa[i][j];
                    }
                }
            }
            return cantidad;
        }

        static tipo[][] MapaVacio(int col, int fil)
        {
            tipo[][] nuevoMapa = new tipo[col][];
            for (int i = 0; i < col; i++)
            {
                nuevoMapa[i] = new tipo[fil];
            }
            return nuevoMapa;
        }

        static tipo[][] InicializarMapa(int col, int fil, int porcentage)
        {
            tipo[][] nuevoMapa = MapaVacio(col,fil);

            for (int i = 0; i < col; i++)
            {
                for (int j = 0; j < fil; j++)
                {
                    int res = Convert.ToInt32(generador.Next(1, 101) <= porcentage);
                    nuevoMapa[i][j] = (tipo)res;
                    cantParedes += res;
                }
            }
            return nuevoMapa;
        }

        static tipo[][] AplicarRegla4(tipo[][] map, int cantidadMinimaPared, int cantidadMaximaVacio, int radioPared, int radioVacio)
        {
            int col = map.Length;
            int fil = map[0].Length;
            tipo[][] nuevoMapa = MapaVacio(col, fil);

            cantParedes = 0;

            for (int j = 0; j < fil; j++)
            {
                for (int i = 0; i < col; i++)
                {
                    switch (map[i][j])
                    {
                        case tipo.vacio:
                            nuevoMapa[i][j] = (tipo)Convert.ToInt32(CantidadVecinosPared(i, j, radioPared, radioPared) >= cantidadMinimaPared + 1 || CantidadVecinosPared(i, j, radioVacio, radioVacio) <= cantidadMaximaVacio);
                            break;
                        case tipo.pared:
                            nuevoMapa[i][j] = (tipo)Convert.ToInt32(CantidadVecinosPared(i, j, radioPared, radioPared) >= cantidadMinimaPared);
                            break;
                    }
                    cantParedes += (int)nuevoMapa[i][j];
                }
            }
            return nuevoMapa;
        }

        static void Explosion(int x, int y, int radioMaximo)
        {
            int scope = generador.Next(1, radioMaximo + 1);

            int startX = x - scope;
            int endX = x + scope + 1;

            for (int i = startX; i < endX; i++)
            {
                scope = generador.Next(1, radioMaximo + 1);
                for (int j = 0 ; j < scope; j++)
                {
                    if (PosValida(i, y + j))
                    {
                        cantParedes -= (int)mapa[i][y + j];
                        mapa[i][y + j] = tipo.vacio;
                    }
                    if (PosValida(i, y - j))
                    {
                        cantParedes -= (int)mapa[i][y - j];
                        mapa[i][y - j] = tipo.vacio;
                    }
                }
            }

        }

        static void Explosiones(int cantidad, int radioMaximo)
        {
            for (int i = 0; i < cantidad; i++)
            {
                Explosion(generador.Next(columnas), generador.Next(filas), radioMaximo);
            }
        }

        static void Mostrar()
        {
            int col = mapa.Length;
            int fil = mapa[0].Length;

            string texto = "";
            for (int j = 0; j < fil; j++)
            {
                for (int i = 0; i < col; i++)
                {
                    switch (mapa[i][j])
                    {
                        case tipo.vacio:
                            texto += ".";
                            break;
                        case tipo.pared:
                            texto += "#";
                            break;
                    }
                }
                texto += "\n";
            }
            Console.WriteLine(texto);
        }

        static bool MapasIguales(tipo[][] mapaViejo)
        {
            for (int i = 0; i < columnas; i++)
            {
                for (int j = 0; j < filas; j++)
                {
                    if (mapa[i][j]!=mapaViejo[i][j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
