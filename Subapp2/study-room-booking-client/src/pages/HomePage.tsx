import { Link } from 'react-router-dom'

import { useAuth } from '../context/AuthContext'

function HomePage() {
    const { isAuthenticated, isAdmin, fullName } = useAuth()

    return (
        <main>
            <section className="container py-5">
                <div className="row align-items-center mb-5">
                    <div className="col-lg-6 mb-5 mb-lg-0">
                        <h1 className="display-5 fw-bold mb-4">
                            Finn studierom og møt andre studenter{' '}
                            <span style={{ color: '#b39700' }}>
                                på samme kurs.
                            </span>
                        </h1>

                        <p className="text-secondary mb-4">
                            Finn ledige studierom, reserver et tidspunkt
                            som passer deg, og gjør studiehverdagen
                            enklere. Enten du vil studere alene eller
                            sammen med andre, finner du et rom som
                            dekker behovet ditt.
                        </p>

                        <div className="d-flex flex-wrap gap-3">
                            {isAuthenticated ? (
                                isAdmin ? (
                                    <Link
                                        to="/rooms"
                                        className="btn btn-lg fw-semibold"
                                        style={{
                                            backgroundColor: '#ffd500',
                                            color: '#212529',
                                            borderColor: '#ffd500',
                                        }}
                                    >
                                        Administrer rom
                                    </Link>
                                ) : (
                                    <span className="text-secondary fst-italic">
                                        Booking av studierom kommer
                                        snart
                                    </span>
                                )
                            ) : (
                                <>
                                    <Link
                                        to="/register"
                                        className="btn btn-lg fw-semibold"
                                        style={{
                                            backgroundColor: '#ffd500',
                                            color: '#212529',
                                            borderColor: '#ffd500',
                                        }}
                                    >
                                        Registrer deg
                                    </Link>
                                    <Link
                                        to="/login"
                                        className="btn btn-outline-dark btn-lg"
                                    >
                                        Logg inn
                                    </Link>
                                </>
                            )}
                        </div>
                    </div>

                    <div className="col-lg-6">
                        <div
                            className="rounded-3 overflow-hidden shadow-sm"
                            style={{
                                height: 450,
                                backgroundColor: '#f0e5a5',
                            }}
                        />
                    </div>
                </div>

                <div className="text-center mb-4">
                    <h2 className="fw-bold">
                        {isAuthenticated
                            ? `Velkommen tilbake, ${fullName}`
                            : 'Gjør studiehverdagen enklere'}
                    </h2>
                    <p className="text-secondary">
                        Finn medstudenter, book studierom, og planlegg
                        studieøktene dine.
                    </p>
                </div>

                <div className="row g-4">
                    <div className="col-md-4">
                        <div className="card h-100 border rounded-3 shadow-sm">
                            <div className="card-body p-4">
                                <div
                                    className="mb-3"
                                    style={{
                                        width: 35,
                                        height: 4,
                                        backgroundColor: '#ffd500',
                                    }}
                                />
                                <h5 className="fw-bold">
                                    Finn et studierom
                                </h5>
                                <p className="text-secondary mb-0">
                                    Se ledige rom og finn et som passer
                                    for deg og gruppen din.
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="col-md-4">
                        <div className="card h-100 border rounded-3 shadow-sm">
                            <div className="card-body p-4">
                                <div
                                    className="mb-3"
                                    style={{
                                        width: 35,
                                        height: 4,
                                        backgroundColor: '#ffd500',
                                    }}
                                />
                                <h5 className="fw-bold">
                                    Book en studieøkt
                                </h5>
                                <p className="text-secondary mb-0">
                                    Velg rom, dato og tidspunkt for
                                    neste studieøkt.
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="col-md-4">
                        <div className="card h-100 border rounded-3 shadow-sm">
                            <div className="card-body p-4">
                                <div
                                    className="mb-3"
                                    style={{
                                        width: 35,
                                        height: 4,
                                        backgroundColor: '#ffd500',
                                    }}
                                />
                                <h5 className="fw-bold">
                                    Oversikt over bookinger
                                </h5>
                                <p className="text-secondary mb-0">
                                    Se alle reservasjonene dine samlet
                                    på ett sted.
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

                {!isAuthenticated && (
                    <section
                        className="d-flex flex-column flex-md-row justify-content-between align-items-md-center gap-4 rounded-3 p-4 p-md-5 mt-5"
                        style={{
                            backgroundColor: '#fff9db',
                            border: '1px solid #f0e5a5',
                        }}
                    >
                        <div>
                            <h2 className="fw-bold mb-3">
                                Klar for din neste studieøkt?
                            </h2>
                            <p className="text-secondary mb-0">
                                Registrer deg og finn et ledig rom i
                                dag.
                            </p>
                        </div>
                        <Link
                            to="/register"
                            className="btn btn-lg fw-semibold text-nowrap"
                            style={{
                                backgroundColor: '#ffd500',
                                color: '#212529',
                                borderColor: '#ffd500',
                            }}
                        >
                            Kom i gang
                        </Link>
                    </section>
                )}
            </section>
        </main>
    )
}

export default HomePage